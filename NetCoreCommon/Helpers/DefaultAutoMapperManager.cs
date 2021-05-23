using AutoMapper;
using NetCoreCommon.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreCommon.Helpers
{
    public class DefaultAutoMapperManager<TProfileID> : IAutoMapperManager<TProfileID>
    {
        private readonly bool _defaultIsSet;
        private readonly IMapper _defaultMapper;

        public DefaultAutoMapperManager(IDictionary<TProfileID, IMapper> profiles, TProfileID defaultProfile,
                                        bool usingUniqueProfileAsDefault = true)
        {
            _defaultIsSet = false;
            Profile = profiles != null ? profiles : new Dictionary<TProfileID, IMapper>();
            if (profiles.Count < 1)
                return;
            if (profiles.Count == 1 & usingUniqueProfileAsDefault)
            {
                _defaultMapper = Profile.First().Value;
                _defaultIsSet = true;
            }
            else
            {
                _defaultMapper = Profile.ContainsKey(defaultProfile) ? Profile[defaultProfile] : 
                                    throw new IndexOutOfRangeException("The AutoMapper profile list does not have a profile with the identifier specified by default");
                _defaultIsSet = true;
            }
        }

        public IMapper Default
        {
            get
            {
                if (_defaultIsSet)
                    return _defaultMapper;

                throw new InvalidOperationException("A default profile has not been established. Please review the service configuration and verify if the list of profiles has at least one");
            }
        }

        public IDictionary<TProfileID, IMapper> Profile { get; }
    }
}
