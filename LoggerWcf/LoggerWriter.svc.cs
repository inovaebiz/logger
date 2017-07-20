using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public bool FazerLog(string DadosLog, string SegundoDados, int EnumTipoLog)
        {
            try
            {
                //Se o valor estiver fora do Intervalo dos Enums, atribui o valor 1 para padronizar como um log de informação
                if (EnumTipoLog < 1 || EnumTipoLog > 3)
                {
                    EnumTipoLog = 1;
                }

                new Logger.Logger().FazerLogAsync(DadosLog, SegundoDados, true, (Logger.EnumTiposDeLog.TiposDeLog)EnumTipoLog);

                return true;
            }
            catch (Exception ex)
            {
                SalvarExceptionRegistro(ex, "LOGGER WS - Sistema de Log", "FazerLog(DadosLog, SegundoDados, EnumTipoLog");
            }

            return false;
        }

        public bool FazerLog(string DadosLog, int EnumTipoLog)
        {
            return FazerLog(DadosLog, "Log simples do webservice", EnumTipoLog);
        }

        public static void SalvarExceptionRegistro(Exception ex, string source, string metodo)
        {
            try
            {
                EventLog eventoLog = new EventLog();
                eventoLog.Source = source;
                eventoLog.WriteEntry("O método " + metodo + " - falhou. Mensagem da exception: " + ex.Message + "\n" +
                    "Tipo da Exception: " + ex.GetType().ToString() + "\n Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
            }
            catch
            {
            }
        }

    }
}
