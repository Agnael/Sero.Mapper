namespace Sero.Mapper
{
    public abstract class AbstractMappingSheet
    {
        public AbstractMappingSheet(IMapperBuilder builder)
        {
            this.MappingRegistration(builder);
        }

        public abstract void MappingRegistration(IMapperBuilder builder);
    }
}