using System;
using System.Collections.Generic;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales
{
    public class ErrorMessage
    {
        private ErrorMessage() { }

        public string ProcessName { get; private set; }
        public string SourceSystem { get; private set; }
        public string TargetSystem { get; private set; }
        public string ExceptionType { get; private set; }
        public string EmailSubject { get; private set; }
        public string DefaultResolverGroup { get; private set; }
        public string DefaultEmailCc { get; private set; }
        public string DefaultEmailTo { get; private set; }
        public string DefaultSeverity { get; private set; }
        public string DefaultFail { get; private set; }
        public string IntErrCode { get; private set; }
        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }
        public List<FaultDetail> FaultDetails { get; private set; }
        public List<AttachBO> AttachBOs { get; private set; }

        public static ErrorMessage Create()
        {
            return new ErrorMessage();
        }

        public void SetProcessName(string value) => ProcessName = value;
        public void SetSourceSystem(string value) => SourceSystem = value;
        public void SetTargetSystem(string value) => TargetSystem = value;
        public void SetExceptionType(string value) => ExceptionType = value;
        public void SetEmailSubject(string value) => EmailSubject = value;
        public void SetDefaultResolverGroup(string value) => DefaultResolverGroup = value;
        public void SetDefaultEmailCc(string value) => DefaultEmailCc = value;
        public void SetDefaultEmailTo(string value) => DefaultEmailTo = value;
        public void SetDefaultSeverity(string value) => DefaultSeverity = value;
        public void SetDefaultFail(string value) => DefaultFail = value;
        public void SetIntErrCode(string value) => IntErrCode = value;
        public void SetClientId(string value) => ClientId = value;
        public void SetClientSecret(string value) => ClientSecret = value;
        public void SetFaultDetails(List<FaultDetail> value) => FaultDetails = value;
        public void SetAttachBOs(List<AttachBO> value) => AttachBOs = value;
    }

    public class FaultDetail
    {
        private FaultDetail() { }

        public string Name { get; private set; }
        public string Value { get; private set; }

        public static FaultDetail Create()
        {
            return new FaultDetail();
        }

        public void SetName(string value) => Name = value;
        public void SetValue(string value) => Value = value;
    }

    public class AttachBO
    {
        private AttachBO() { }

        public string BoName { get; private set; }
        public string Data { get; private set; }

        public static AttachBO Create()
        {
            return new AttachBO();
        }

        public void SetBoName(string value) => BoName = value;
        public void SetData(string value) => Data = value;
    }
}
