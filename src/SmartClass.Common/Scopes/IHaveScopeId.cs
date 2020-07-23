namespace SmartClass.Common.Scopes
{
    public interface IHaveScopeId
    {
        /// <summary>
        /// 逻辑上划分的范围
        /// </summary>
        string ScopeId { get; set; }
    }
}
