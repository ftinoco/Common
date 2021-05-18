using AutoMapper;
using System.Collections.Generic;

namespace NetCoreCommon.AutoMapper
{
    public interface IAutoMapperManager<TProfileID>
    {
        IMapper Default { get; }
        IDictionary<TProfileID, IMapper> Profile { get; }
    }
}
