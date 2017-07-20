using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;

namespace LoggerWcf
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da classe "Service1" no arquivo de código, svc e configuração ao mesmo tempo.
    // OBSERVAÇÃO: Para iniciar o cliente de teste do WCF para testar esse serviço, selecione Service1.svc ou Service1.svc.cs no Gerenciador de Soluções e inicie a depuração.
    public class LoggerWriter : ILoggerWriter
    {
        static Logger.Logger log;

        static LoggerWriter()
        {
            log = new Logger.Logger();
        }

        public LoggerWriter() { }

        public async void FazerLog(string DadosLog, string SegundoDados, int EnumTipoLog)
        {
            try
            {
                //Se o valor estiver fora do Intervalo dos Enums, atribui o valor 1 para padronizar como um log de informação
                if (EnumTipoLog < 0 || EnumTipoLog > 2)
                {
                    EnumTipoLog = 2;
                }

                log = new Logger.Logger();
                log.FazerLogAsync(DadosLog.Replace("\\",""), SegundoDados.Replace("\\", ""), true, (Logger.EnumTiposDeLog.TiposDeLog)EnumTipoLog);

                //Thread.Sleep(5000);

            }
            catch (Exception ex)
            {
                SalvarExceptionRegistro(ex, "LOGGER WS - Sistema de Log", "FazerLog(DadosLog, SegundoDados, EnumTipoLog");
            }

        }

        public async void FazerLogSimples(string DadosLog, int EnumTipoLog)
        {
            FazerLog(DadosLog, "Log simples do webservice", EnumTipoLog);
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
