namespace Healthcheck.Service.Core
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reflection;
    using DateTime = System.DateTime;

    public class CustomCheck
    {
        public static HealthcheckResult RunHealthcheck(string typeValue, NameValueCollection inputParameters)
        {
            var checkResult = new HealthcheckResult
            {
                LastCheckTime = System.DateTime.UtcNow,
                Status = Customization.HealthcheckStatus.Healthy,
                ErrorList = new ErrorList
                {
                    Entries = new List<ErrorEntry>()
                }
            };

            string assemblyName = string.Empty;
            string typeName = string.Empty;

            try
            {
                assemblyName = typeValue.Split(',')[1].Trim();
                typeName = typeValue.Split(',')[0].Trim();
            }
            catch (Exception)
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = string.Format("{0} could not be parsed, use the following format: <NameSpace>.<Class>,<AssemblyName>", typeValue),
                    ErrorLevel = ErrorLevel.Warning
                });

                return checkResult;
            }

            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch (Exception ex)
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = string.Format("{0} assembly could not be loaded, please check the configuration!", assemblyName),
                    Exception = ex,
                    ErrorLevel = ErrorLevel.Warning
                });

                return checkResult;
            }

            Type type = assembly.GetType(typeName);

            if (type == null)
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = string.Format("{0} type could not be loaded, please check the configuration!", typeName),
                    ErrorLevel = ErrorLevel.Warning
                });

                return checkResult;
            }

            var methodName = "DoHealthcheck";

            MethodInfo methodInfo = type.GetMethod(methodName);

            if (methodInfo == null)
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = string.Format("{0} method could not be loaded, please make sure you implemented the 'DoHealthcheck' method!", methodName),
                    ErrorLevel = ErrorLevel.Warning
                });

                return checkResult;
            }

            try
            {
                CustomHealthcheckResult result = null;
                ParameterInfo[] parameters = methodInfo.GetParameters();
                object classInstance = Activator.CreateInstance(type, null);

                object[] parametersArray = new object[] { inputParameters };

                result = methodInfo.Invoke(classInstance, parametersArray) as CustomHealthcheckResult;

                if (result != null)
                {
                    checkResult.Status = result.Status;
                    checkResult.HealthyMessage = result.HealthyMessage;
                    if (checkResult.Status != HealthcheckStatus.Healthy)
                    {
                        checkResult.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = DateTime.UtcNow,
                            Reason = result.ErrorMessage,
                            Exception = result.Exception,
                            ErrorLevel = result.ErrorLevel
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                checkResult.Status = HealthcheckStatus.Error;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = ex.Message,
                    Exception = ex,
                    ErrorLevel = ErrorLevel.Error
                });
            }

            return checkResult;
        }
    }
}