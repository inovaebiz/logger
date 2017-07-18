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
using System.Threading.Tasks;

namespace LoggerWriterWCF
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da classe "Service1" no arquivo de código, svc e configuração ao mesmo tempo.
    // OBSERVAÇÃO: Para iniciar o cliente de teste do WCF para testar esse serviço, selecione Service1.svc ou Service1.svc.cs no Gerenciador de Soluções e inicie a depuração.
    public class LoggerWriter : ILoggerWriter
    {
        private string caminhoDoLog = string.Empty;

        private static string caminhoDoLogEstatico = string.Empty;
        private static string caminhoDoLogEstaticoAlerta = string.Empty;
        private static string caminhoDoLogEstaticoErro = string.Empty;

        //GUID, (log,caminho)
        private static Dictionary<Guid, Tuple<string, string>> dictLogs = new Dictionary<Guid, Tuple<string, string>>();

        private static bool escrevendoLog = false;


        public bool EscreverLog(string log, string caminho)
        {
            try
            {
                if (!escrevendoLog)
                {
                    escrevendoLog = true;
                    Thread.Sleep(new Random().Next(1200, 4000));
                    WriteAsync();
                }
                dictLogs.Add(Guid.NewGuid(), new Tuple<string, string>(log, caminho));

            }
            catch (Exception ex)
            {
                SalvarExceptionRegistro(ex);
            }

            return true;
        }

        public void SalvarExceptionRegistro(Exception ex)
        {
            try
            {
                EventLog eventoLog = new EventLog();
                eventoLog.Source = "LOGGER WRITER WCF - Sistema de Log";
                eventoLog.WriteEntry("O método EscreverLog() - Escrever um registro - falhou. Mensagem da exception: " + ex.Message + "/// Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
            }
            catch
            {
            }
        }

        public async void WriteAsync()
        {
            var tr = new Task<bool>(() => WriteLog());
            tr.Start();
            bool resposta = false;
            resposta = await tr;
            tr.Dispose();
        }

        private bool WriteLog()
        {
            for (long i = 0; i <= Int64.MaxValue; i++)
            {
                Thread.Sleep(5000);

                if (dictLogs.Count > 0)
                {
                    string linharemovida = string.Empty;

                    var linhasAtual = dictLogs.Take(1000).ToList();

                    foreach (var item in linhasAtual)
                    {
                        try
                        {
                            File.AppendAllText(item.Value.Item2, item.Value.Item1 + Environment.NewLine + "," + Environment.NewLine);
                            dictLogs.Remove(item.Key);
                        }
                        catch (Exception ex)
                        {
                            SalvarExceptionRegistro(ex);
                        }
                    }
                }
            }

            return true;
        }
    }


}