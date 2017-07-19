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
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da classe "Service1" no arquivo de código, svc e configuração ao mesmo tempo.
    // OBSERVAÇÃO: Para iniciar o cliente de teste do WCF para testar esse serviço, selecione Service1.svc ou Service1.svc.cs no Gerenciador de Soluções e inicie a depuração.
    public class LoggerWriter : ILoggerWriter
    {
        public bool FazerLog(string data)
        {
            /*MemoryStream ms = new MemoryStream();
            DadosLog.CopyTo(ms);
            var info = ms.ToArray();*/
            

            new Logger.Logger().FazerLogAsync(data, "");

            return true;
        }

        public bool FazerLog(string DadosLog, string SegundoDados, int EnumTipoLog)
        {
            new Logger.Logger().FazerLogAsync(DadosLog, SegundoDados, (Logger.EnumTiposDeLog.TiposDeLog)EnumTipoLog);

            return true;
        }

        public bool FazerLog<T>(T DadosLog, dynamic SegundoDados, int EnumTipoLog)
        {
            new Logger.Logger().FazerLogAsync(DadosLog, SegundoDados, (Logger.EnumTiposDeLog.TiposDeLog)EnumTipoLog);

            return true;
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }


        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
