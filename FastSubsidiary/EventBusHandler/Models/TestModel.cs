namespace Extensions.EventBusHandler.Models
{
    /// <summary>
    /// 删除接口事件的参数类型
    /// </summary>
    public class TestModel
    {
        public string id { get; private set; }

        public TestModel(string id) => this.id = id;
    }
}