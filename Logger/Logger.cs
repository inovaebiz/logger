using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Logger
{
    public class Logger
    {
        private string caminhoDoLog = string.Empty;

        private static string caminhoDoLogStatic = string.Empty;

        private static Dictionary<Guid, string> linhasEscrever = new Dictionary<Guid, string>();

        private static bool escrevendoLog = false;

        public Logger()
        {
            if (!Logger.escrevendoLog)
            {
                Logger.escrevendoLog = true;
                CriarArquivoFisicoLog();
                Logger.LogWriterAsync();
            }
        }

        private static async void LogWriterAsync()
        {
            var tr = new Task<bool>(() => IniciarEscreverLog());
            tr.Start();
            var abc = await tr;
            tr.Dispose();
        }

        private static bool IniciarEscreverLog()
        {
            try
            {
                for (long i = 0; i < Int64.MaxValue; i++)
                {
                    Thread.Sleep(5000);

                    if (linhasEscrever.Count > 0)
                    {
                        var linhasAtual = linhasEscrever.Take(1000).ToList();

                        foreach (var item in linhasAtual)
                        {
                            try
                            {
                                File.AppendAllText(caminhoDoLogStatic, item.Value + Environment.NewLine);
                                linhasEscrever.Remove(item.Key);
                            }
                            catch (Exception ex)
                            {
                                EventLog eventoLog = new EventLog();
                                eventoLog.Source = "LOGGER - Sistema de Log";
                                eventoLog.WriteEntry("O método IniciarEscreverLog() - Escrever um registro - falhou. Mensagem da exception: " + ex.Message + "/// Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                escrevendoLog = false;
                EventLog eventoLog = new EventLog();
                eventoLog.Source = "LOGGER - Sistema de Log";
                eventoLog.WriteEntry("O método IniciarEscreverLog() falhou. Mensagem da exception: " + ex.Message + "/// Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Método de chamada para que se inicie o processo de log
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DadosLog"></param>
        /// <returns></returns>
        public bool FazerLog<T>(T DadosLog)
        {
            try
            {
                bool ArquivoCriado = CriarArquivoFisicoLog();

                if (ArquivoCriado)
                {
                    PreencherLog(DadosLog);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                EventLog eventoLog = new EventLog();
                eventoLog.Source = "LOGGER - Sistema de Log";
                eventoLog.WriteEntry("O método FAZERLOG(T DadosLog) falhou. Mensagem da exception: " + ex.Message + "/// Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
                return false;
            }
        }

        public bool FazerLog<T>(T DadosLog, dynamic SegundoDados)
        {
            try
            {
                bool ArquivoCriado = CriarArquivoFisicoLog();

                if (ArquivoCriado)
                {
                    PreencherLog(DadosLog, SegundoDados);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                EventLog eventoLog = new EventLog();
                eventoLog.Source = "LOGGER - Sistema de Log";
                eventoLog.WriteEntry("O método FAZERLOG(T DadosLog, dynamic SegundoDados) falhou. Mensagem da exception: " + ex.Message + "/// Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
                return false;
            }
        }

        public async void FazerLogAsync<T>(T DadosLog, dynamic SegundoDados)
        {
            var tr = new Task<bool>(() => FazerLog(DadosLog, SegundoDados));
            tr.Start();
            var abc = await tr;
            tr.Dispose();
        }

        ///// <summary>
        ///// Override quando tiver itens a mais do que o carrinho a ser colocado no log
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="DadosLog"></param>
        ///// <param name="RequisicoesXml"></param>
        ///// <returns></returns>
        //public bool FazerLog<T>(T DadosLog, Dictionary<object, object> DicionarioDadosLog)
        //{
        //    try
        //    {
        //        bool ArquivoCriado = CriarArquivoFisicoLog();

        //        if (ArquivoCriado)
        //        {
        //            PreencherLog(DadosLog, DicionarioDadosLog);
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLog eventoLog = new EventLog();
        //        eventoLog.Source = "LOGGER - Sistema de Log";
        //        eventoLog.WriteEntry("O método FAZERLOG(T DadosLog, Dictionary<object, object> DicionarioDadosLog) falhou. Mensagem da exception: " + ex.Message + "/// Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
        //        return false;
        //    }
        //}

        /// <summary>
        /// Override do método para que a chamada seja mais simplificado para uso do usuario. Que ao inves de enviar um dicionario montado, o usuario envia o Titula do ação e depois a Mensagem da ação.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DadosLog"></param>
        /// <param name="TituloAcao"></param>
        /// <param name="MensagemAcao"></param>
        /// <returns></returns>
        public bool FazerLog<T>(T DadosLog, string TituloAcao, string MensagemAcao)
        {
            try
            {
                bool ArquivoCriado = CriarArquivoFisicoLog();

                if (ArquivoCriado)
                {
                    Dictionary<object, object> DicionarioDadosLog = new Dictionary<object, object>();
                    DicionarioDadosLog.Add(TituloAcao, MensagemAcao);
                    PreencherLog(DadosLog, DicionarioDadosLog);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                EventLog eventoLog = new EventLog();
                eventoLog.Source = "LOGGER - Sistema de Log";
                eventoLog.WriteEntry("O método FAZERLOG(T DadosLog,string TituloAcao, string MensagemAcao) falhou. Mensagem da exception: " + ex.Message + "/// Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
                return false;
            }
        }

        /// <summary>
        /// Método que faz a criação do arquivo fisico do Log
        /// </summary>
        /// <returns></returns>
        private bool CriarArquivoFisicoLog()
        {
            try
            {
                if (WebConfigurationManager.AppSettings["CaminhoLog"] != null)
                {
                    string nomeDoArquivo = "Logger_" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "_" + DateTime.Today.ToString("dd-MM-yyyy") + ".json";
                    caminhoDoLog = WebConfigurationManager.AppSettings["CaminhoLog"].ToString() + nomeDoArquivo;
                    Logger.caminhoDoLogStatic = caminhoDoLog;
                }
                else
                {
                    string nomeDoArquivo = "Logger_" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "_" + DateTime.Today.ToString("dd-MM-yyyy") + ".json";
                    caminhoDoLog = System.AppDomain.CurrentDomain.BaseDirectory + "Logger\\";
                    Logger.caminhoDoLogStatic = caminhoDoLog;
                    if (!Directory.Exists(caminhoDoLog))
                    {
                        Directory.CreateDirectory(caminhoDoLog);
                    }
                    caminhoDoLog = caminhoDoLog + nomeDoArquivo;
                    Logger.caminhoDoLogStatic = caminhoDoLog;
                }

                if (ValidarArquivo(caminhoDoLog))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                EventLog eventoLog = new EventLog("");
                eventoLog.Source = "LOGGER - Sistema de Log";
                eventoLog.WriteEntry("Preenchimento do log falhou. Mensagem da exception: " + ex.Message + "/// Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
                return false;
            }
        }

        /// <summary>
        /// Método que faz a validação se o arquivo existe ou não no sistema
        /// </summary>
        /// <param name="caminhoDoLog"></param>
        /// <returns></returns>
        private bool ValidarArquivo(string caminhoDoLog)
        {
            try
            {
                if (File.Exists(caminhoDoLog))
                {
                    return true;
                }
                else
                {
                    //File.Create(caminhoDoLog);
                    //Trocado pelo método de AppendAllText pois se o arquivo não existe ele já tenta criar o mesmo
                    //Isso pode ajudar nos casos da chamada do método acontecer duas vezes seguidas
                    File.AppendAllText(caminhoDoLog, " ");
                    EfetuarLimpezaSemanal();
                    return true;
                }
            }
            catch (Exception ex)
            {
                EventLog eventoLog = new EventLog("");
                eventoLog.Source = "LOGGER - Sistema de Log";
                eventoLog.WriteEntry("Criação do arquivo falhou. Mensagem da exception: " + ex.Message + "/// Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
                return false;
            }
        }

        private void EfetuarLimpezaSemanal()
        {
            try
            {
                string caminhoLog = "";

                if (WebConfigurationManager.AppSettings["CaminhoLog"] != null)
                {
                    string nomeDoArquivo = "Logger_" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "_" + DateTime.Today.AddDays(-7).ToString("dd-MM-yyyy") + ".json";
                    caminhoLog = WebConfigurationManager.AppSettings["CaminhoLog"].ToString() + nomeDoArquivo;
                }
                else
                {
                    string nomeDoArquivo = "Logger_" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "_" + DateTime.Today.AddDays(-7).ToString("dd-MM-yyyy") + ".json";
                    caminhoLog = System.AppDomain.CurrentDomain.BaseDirectory + "Logger\\";
                    caminhoLog = caminhoDoLog + nomeDoArquivo;
                }

                File.Delete(caminhoLog);
            }
            catch (Exception ex)
            {
                EventLog eventoLog = new EventLog("");
                eventoLog.Source = "LOGGER - Sistema de Log";
                eventoLog.WriteEntry("Limpeza semanal de arquivos falhou. Mensagem da exception: " + ex.Message + "/// Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Método assincrono que faz o preenchimento do log no arquivo já validado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DadosParaLog"></param>
        private void PreencherLog<T>(T DadosParaLog)
        {
            try
            {
                string Dadosjson = JsonConvert.SerializeObject(new { InformacoesLog = DadosParaLog, DataDoLog = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Include
                    });
                Logger.linhasEscrever.Add(Guid.NewGuid(), Dadosjson);
            }
            catch (Exception ex)
            {
                EventLog eventoLog = new EventLog();
                eventoLog.Source = "LOGGER - Sistema de Log";
                eventoLog.WriteEntry("Preenchimento do log falhou. Mensagem da exception: " + ex.Message + "/// Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Override para quando ocorrer o Dicionario de dados ele gerar o log do mesmo.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dadosLog"></param>
        /// <param name="DicionarioDadosLog"></param>
        private void PreencherLog<T>(T dadosLog, Dictionary<object, object> DicionarioDadosLog)
        {
            try
            {
                string Dadosjson = JsonConvert.SerializeObject(new { InformacoesPrincipaisLog = dadosLog, InformacoesAdicionais = DicionarioDadosLog, DataDoLog = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Include
                    });
                Logger.linhasEscrever.Add(Guid.NewGuid(), Dadosjson);
            }
            catch (Exception ex)
            {
                EventLog eventoLog = new EventLog();
                eventoLog.Source = "LOGGER - Sistema de Log";
                eventoLog.WriteEntry("Preenchimento do log falhou. Mensagem da exception: " + ex.Message + "/// Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Override para quando ocorrer de ter um objeto dinamico de dados ele gerar o log do mesmo.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dadosLog"></param>
        /// <param name="DicionarioDadosLog"></param>
        private void PreencherLog<T>(T DadosParaLog, dynamic InformacoesAdicionais)
        {
            try
            {
                string Dadosjson = JsonConvert.SerializeObject(new { InformacoesLog = DadosParaLog, InformacoesAdicionais, DataDoLog = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    });
                Logger.linhasEscrever.Add(Guid.NewGuid(), Dadosjson);
            }
            catch (Exception ex)
            {
                EventLog eventoLog = new EventLog();
                eventoLog.Source = "LOGGER - Sistema de Log";
                eventoLog.WriteEntry("Preenchimento do log falhou.\n Mensagem da exception: " + ex.Message + "\n" +
                    "Tipo da Exception: " + ex.GetType().ToString() + "\n Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
            }
        }
    }
}