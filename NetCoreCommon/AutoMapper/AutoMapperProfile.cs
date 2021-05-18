using AutoMapper;

namespace NetCoreCommon.AutoMapper
{
    public abstract class AutoMapperProfile<TProfileID>
    {
        public AutoMapperProfile(TProfileID profileID)
        {
            ProfileID = profileID;
        }

        public TProfileID ProfileID { get; set; }

        public abstract void ConfigureProfile(IMapperConfigurationExpression configuration);
    }
}
