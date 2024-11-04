
namespace Game.Thirteen
{
    using System.Collections.Generic;
    public interface IFilter<TResult>
    {
        public List<TResult> RunFilter();
    }
}
