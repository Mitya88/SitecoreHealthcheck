namespace Healthcheck.Service
{
    using Sitecore.Pipelines;
    using System.Web.Http;
    using System.Web.Routing;

    public class RegisterHttpRoutes
    {
        public virtual void Process(PipelineArgs args)
        {
            RegisterRoute(RouteTable.Routes);
        }

        protected virtual void RegisterRoute(RouteCollection routes)
        {
            RouteTable.Routes.MapHttpRoute("Healthcheck",
                "sitecore/api/ssc/healthcheck/{action}",
                new { controller = "HealthcheckApi" });

            RouteTable.Routes.MapHttpRoute("HealthcheckComponent",
                "sitecore/api/ssc/healthcheck/{action}/{id}",
                new { controller = "HealthcheckApi" });

            RouteTable.Routes.MapHttpRoute("HealthcheckErrors",
                "sitecore/api/ssc/errors/{action}",
                new { controller = "ErrorsApi" });
        }
    }
}