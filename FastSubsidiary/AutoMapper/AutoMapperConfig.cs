using AutoMapper;

namespace Extensions.AutoMapper
{
    /// <summary>
    /// AutoMapper 配置文件
    /// </summary>
    public class AutoMapperConfig
    {
        /// <summary>
        /// 配置实体类的映射关系
        /// </summary>
        /// <returns></returns>
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(configure =>
            {
                configure.AddProfile(new CustomProfile());//添加一个关系映射
            });
        }
    }
}