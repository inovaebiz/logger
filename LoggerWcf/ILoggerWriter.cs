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
        [OperationContract]
        [WebGet(UriTemplate = "/Escrever?log={DadosLog}&second={SegundoDados}&TipoLog={EnumTipoLog}",
           BodyStyle = WebMessageBodyStyle.Bare,
           RequestFormat = WebMessageFormat.Json)]
        bool FazerLog(string DadosLog, string SegundoDados, int EnumTipoLog);

        [OperationContract]
        [WebGet(UriTemplate = "/Escrever?log={DadosLog}&TipoLog={EnumTipoLog}",
           BodyStyle = WebMessageBodyStyle.Bare,
           RequestFormat = WebMessageFormat.Json)]
        bool FazerLog(string DadosLog, int EnumTipoLog);

        // TODO: Adicione suas operações de serviço aqui
    }

}
