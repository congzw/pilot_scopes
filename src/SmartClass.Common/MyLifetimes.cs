namespace SmartClass
{
    public interface IMyLifetime
    {
    }

    public interface IMySingleton: IMyLifetime
    {

    }

    public interface IMyScoped: IMyLifetime
    {

    }

    public interface IMyTransient: IMyLifetime
    {

    }
}

#region for demo only

namespace SmartClass.Common.Demos
{
    public interface ILifetimeDesc : IMyLifetime
    {

    }

    public interface IMySingletonDesc : ILifetimeDesc
    {

    }

    public interface IMyScopedDesc : ILifetimeDesc
    {

    }

    public interface IMyTransientDesc : ILifetimeDesc
    {

    }

    public class LifetimeDesc : IMySingletonDesc, IMyScopedDesc, IMyTransientDesc
    {
        public override string ToString()
        {
            return this.GetHashCode().ToString();
        }

        public static string ShowDiff(ILifetimeDesc desc, ILifetimeDesc desc2)
        {
            return string.Format("[{0}, {1}] Same: {2}"
                , desc == null ? "NULL" : desc.ToString()
                , desc2 == null ? "NULL" : desc.ToString()
                , object.ReferenceEquals(desc, desc2));
        }
    }

}

#endregion