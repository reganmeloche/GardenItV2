#include <SimpleTimer.h>
#include <ESP8266WiFi.h>
#include <PubSubClient.h>
#include <Adafruit_seesaw.h>

bool DEBUG = true;

/*** COMMUNICATION SETUP ***/

// MQTT Info: Obtained from cloudMQTT
const char *MQTT_SERVER = "[INSERT DATA]";
const int MQTT_PORT = -1;
const char *MQTT_USER = "[INSERT DATA]";
const char *MQTT_PASSWORD = "[INSERT DATA]";
const char *MQTT_CLIENT_NAME = "[INSERT DATA]";

// MQTT Topics
const char *PUB_TOPIC = "device_call/[UNIT ID]";
const char *SUB_TOPIC = "web_call/[UNIT ID]";

/// WiFi Network credentials
const char* WIFI_SSID = "[INSERT DATA]";
const char* WIFI_PASSWORD = "[INSERT DATA]";

// Variables
int PUMP_PIN = 10;
int MOISTURE_POLL_MS = 30 * 60 * 1000;

// Declarations
int timerId;
SimpleTimer timer;
WiFiClient espClient;
PubSubClient client(MQTT_SERVER, MQTT_PORT, espClient);
Adafruit_seesaw ss;

void callback(char* topic, byte* payload, unsigned int length);

void setup() {
  Serial.begin(115200);
  pinMode(PUMP_PIN, OUTPUT);
  ss.begin(0x36);
  timerId = timer.setInterval(MOISTURE_POLL_MS, readMoisture);
}

void loop() {
  connectWifi();
  connectPubSub();
  if (WiFi.status() == WL_CONNECTED && client.connected()) {
    client.loop();
    timer.run();
  }
}

void connectWifi() {
  if (WiFi.status() != WL_CONNECTED) {
    myPrint("Connecting to ");
    myPrint(WIFI_SSID);
    myPrintln("...");
    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
 
    if (WiFi.waitForConnectResult() != WL_CONNECTED) {
      return;
    }
    myPrintln("WiFi connected");
  }
}

void connectPubSub() {
  if (!client.connected()) {
    myPrintln("Connecting to MQTT server");
      
    if (client.connect(MQTT_CLIENT_NAME, MQTT_USER, MQTT_PASSWORD)) {
       myPrintln("Connected to MQTT server");       
       client.setCallback(callback);
       client.subscribe(SUB_TOPIC);   
    } else {
        myPrintln("Could not connect to MQTT server");   
    }
  }
}

// Executed when a message is read from MQTT
void callback(char* topic, byte* payload, unsigned int len) {  
  printMessage(topic, payload, len);
  parseAndDispatchMessage(payload, len);
}

void parseAndDispatchMessage(byte* payload, unsigned int len) {
  char isRead = ((char)payload[0] == 'R');
  char command = (char)payload[1];
  char value[16] = "";
  
  for (int i = 2; i < len; i++) {
    value[i - 2] = (char)payload[i]; 
  }
  control(isRead, command, value);
}


/*
 * control:
 * Executes the appropriate control based on the given command
 * isRead: True if the command is a "read" command (e.g. reading temp)
 *         False if it is a "write" command (e.g. moving a motor)
 * command: Character that denotes the sensor/actuator being triggered
 *          M = moisture sensor, W = water pump, P = poll period
 * value: String denoting the actual value being sent to an actuator (writing only)
*/
void control(bool isRead, char command, char* value) {
  if (command == 'W') {
    int delayMs = atoi(value);
    triggerPump(delayMs);
  } else if (command == 'M') {
    readMoisture();
    timer.restartTimer(timerId);
  } else if (command == 'P') {
    int pollPeriodMinutes = atoi(value);
    setPollPeriod(pollPeriodMinutes);
  }
}

void triggerPump(int delayMs) {
  digitalWrite(PUMP_PIN, HIGH);
  delay(delayMs);
  digitalWrite(PUMP_PIN, LOW);
}

void readMoisture() {
  float tempC = ss.getTemp();
  uint16_t capread = ss.touchRead(0);
  printMoistureValues(tempC, capread);
  publishMoistureValues(tempC, capread);
}

void publishMoistureValues(float tempC, uint16_t capread) {
  char result[10] = "";
  sprintf(result, "%.2f,%d", tempC, capread);
  char msg[12] = "";
  sprintf(msg,"%s%s", "M", result);
  client.publish(PUB_TOPIC, msg); 
}

void setPollPeriod(int minutes) {
  timer.deleteTimer(timerId);
  MOISTURE_POLL_MS = minutes * 60 * 1000;
  timerId = timer.setInterval(MOISTURE_POLL_MS, readMoisture);
}



/*** PRINTING CENTER ***/

void printMessage(char* topic, byte* payload, unsigned int len) {
  if (DEBUG) {
    Serial.print("Message arrived [");
    Serial.print(topic);
    Serial.print("] ");
    for (int i = 0; i < len; i++) {
      Serial.print((char)payload[i]);
    }
    Serial.println(); 
  }
}

void printMoistureValues(float tempC, uint16_t capread) {
  if (DEBUG) {
    Serial.print("Temperature: "); Serial.print(tempC); Serial.println("*C");
    Serial.print("Capacitive: "); Serial.println(capread);  
  }
}

void myPrint(String message) {
  if (DEBUG) {
    Serial.print(message);
  }
}

void myPrintln(String message) {
  if (DEBUG) {
    Serial.println(message);
  }
}