using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace LoggerWcf
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da interface "IService1" no arquivo de código e configuração ao mesmo tempo.
    [ServiceContract]
    public interface ILoggerWriter
    {
        /*[OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Escrever", BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool FazerLog<T>(T DadosLog, dynamic SegundoDados, int EnumTipoLog);*/

        /*[OperationContract]
        [WebInvoke(Method = "POST", 
                   UriTemplate = "/Escrever", 
                   BodyStyle = WebMessageBodyStyle.Wrapped,
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json)]
        bool FazerLog(dynamic DadosLog, dynamic SegundoDados, int EnumTipoLog);*/

        [OperationContract]
        [WebGet(UriTemplate = "/Escrever?log={DadosLog}&second={SegundoDados}&TipoLog={EnumTipoLog}",
           BodyStyle = WebMessageBodyStyle.Bare,
           RequestFormat = WebMessageFormat.Json)]
        bool FazerLog(string DadosLog, string SegundoDados, int EnumTipoLog);

        // TODO: Adicione suas operações de serviço aqui
    }

    [DataContract]
    public class DynamicEntry
    {

        [DataMember]
        public object DadosLog;
    }

    // Use um contrato de dados como ilustrado no exemplo abaixo para adicionar tipos compostos a operações de serviço.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
