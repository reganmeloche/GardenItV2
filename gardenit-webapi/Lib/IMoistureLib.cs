using System;
using System.Threading.Tasks;

using gardenit_api_classes.Moisture;

namespace gardenit_webapi.Lib
{
    public interface IMoistureLib
    {
        Task RequestReading(MoistureReadingRequest req);
        void AddReading(Guid id, MoistureReading reading);
    }
}