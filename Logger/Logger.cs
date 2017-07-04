using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Logger
{
    public class Logger
    {
        private string caminhoDoLog = string.Empty;

        private static string caminhoDoLogEstatico = string.Empty;

        private static Dictionary<Guid, string> linhasEscrever = new Dictionary<Guid, string>();

        private static bool escrevendoLog = false;

        private bool loggerAtivo = true;

        private List<String> logsExclusivos;

        public Logger()
        {
            if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["Logger.Ativo"]))
            {
                loggerAtivo = Convert.ToBoolean(WebConfigurationManager.AppSettings["Logger.Ativo"]);
            }
            else
            {
                loggerAtivo = false;
            }

            if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["Logger.GravarLogExclusivo"]))
            {
                logsExclusivos = WebConfigurationManager.AppSettings["Logger.GravarLogExclusivo"].Split(',').ToList();
                if (logsExclusivos.Count > 0)
                {
                    for (int i = 0; i < logsExclusivos.Count; i++)
                    {
                        logsExclusivos[i] = logsExclusivos[i].Trim();
                    }
                }
            }
            else
            {
                logsExclusivos = new List<string>();
            }


            if (loggerAtivo)
            {
                if (!Logger.escrevendoLog)
                {
                    Logger.escrevendoLog = true;
                    CriarArquivoFisicoLog();
                    Logger.EscreverLogAssincrono();
                }
            }
            else
            {
                EventLog eventoLog = new EventLog();
                eventoLog.Source = "LOGGER - Sistema de Log";
                eventoLog.WriteEntry("O LOGGER não esta ativido no momento para LOG", EventLogEntryType.Information);
            }
        }

        private static async void EscreverLogAssincrono()
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
                                File.AppendAllText(caminhoDoLogEstatico, item.Value + Environment.NewLine);
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
        public bool FazerLog<T>(T DadosLog, EnumTiposDeLog.TiposDeLog tipoDoLog = EnumTiposDeLog.TiposDeLog.Informacao)
        {
            try
            {
                if (logsExclusivos.Exists(x => x == tipoDoLog.ToString()) || logsExclusivos.Count == 0)
                {
                    bool ArquivoCriado = CriarArquivoFisicoLog(tipoDoLog);

                    if (ArquivoCriado)
                    {
                        PreencherLog(DadosLog, tipoDoLog);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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

        /// <summary>
        /// Override com tipo dynamic como segunda atribuição do metodo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DadosLog"></param>
        /// <param name="SegundoDados"></param>
        /// <returns></returns>
        public bool FazerLog<T>(T DadosLog, dynamic SegundoDados, EnumTiposDeLog.TiposDeLog tipoDoLog = EnumTiposDeLog.TiposDeLog.Informacao)
        {
            try
            {
                if (logsExclusivos.Exists(x => x == tipoDoLog.ToString()) || logsExclusivos.Count == 0)
                {
                    bool ArquivoCriado = CriarArquivoFisicoLog(tipoDoLog);

                    if (ArquivoCriado)
                    {
                        PreencherLog(DadosLog, SegundoDados, tipoDoLog);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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

        /// <summary>
        /// Chamada de metodo assincrono, que cria uma Task para que seja executada.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DadosLog"></param>
        /// <param name="SegundoDados"></param>
        public async void FazerLogAssincrono<T>(T DadosLog, dynamic SegundoDados, EnumTiposDeLog.TiposDeLog tipoDoLog = EnumTiposDeLog.TiposDeLog.Informacao)
        {
            var tr = new Task<bool>(() => FazerLog(DadosLog, SegundoDados, tipoDoLog));
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
        public bool FazerLog<T>(T DadosLog, string TituloAcao, string MensagemAcao, EnumTiposDeLog.TiposDeLog tipoDoLog = EnumTiposDeLog.TiposDeLog.Informacao)
        {
            try
            {
                if (logsExclusivos.Exists(x => x == tipoDoLog.ToString()) || logsExclusivos.Count == 0)
                {

                    bool ArquivoCriado = CriarArquivoFisicoLog(tipoDoLog);

                    if (ArquivoCriado)
                    {
                        Dictionary<object, object> DicionarioDadosLog = new Dictionary<object, object>();
                        DicionarioDadosLog.Add(TituloAcao, MensagemAcao);
                        PreencherLog(DadosLog, DicionarioDadosLog, tipoDoLog);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
        private bool CriarArquivoFisicoLog(EnumTiposDeLog.TiposDeLog tipoDoLog = EnumTiposDeLog.TiposDeLog.Informacao)
        {
            string nomeDoAplicativo;

            try
            {
                if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["Logger.NomeDaAplicacao"]))
                {
                    nomeDoAplicativo = WebConfigurationManager.AppSettings["Logger.NomeDaAplicacao"].ToString();
                }
                else
                {
                    nomeDoAplicativo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                }

                if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["Logger.DiretorioDeGravacao"]))
                {
                    string nomeDoArquivo = "Logger_" + tipoDoLog +"_"+ nomeDoAplicativo + "_" + DateTime.Today.ToString("dd-MM-yyyy") + ".json";
                    caminhoDoLog = WebConfigurationManager.AppSettings["Logger.DiretorioDeGravacao"].ToString() + nomeDoArquivo;
                    Logger.caminhoDoLogEstatico = caminhoDoLog;
                }
                else
                {
                    string nomeDoArquivo = "Logger_" + tipoDoLog + "_" + nomeDoAplicativo + "_" + DateTime.Today.ToString("dd-MM-yyyy") + ".json";
                    caminhoDoLog = System.AppDomain.CurrentDomain.BaseDirectory + "Logger\\";

                    Logger.caminhoDoLogEstatico = caminhoDoLog;
                    if (!Directory.Exists(caminhoDoLog))
                    {
                        Directory.CreateDirectory(caminhoDoLog);
                    }
                    caminhoDoLog = caminhoDoLog + nomeDoArquivo;
                    Logger.caminhoDoLogEstatico = caminhoDoLog;
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
                    EfetuarLimpezaDeArquivos(EnumTiposDeLog.TiposDeLog.Informacao);
                    EfetuarLimpezaDeArquivos(EnumTiposDeLog.TiposDeLog.Alerta);
                    EfetuarLimpezaDeArquivos(EnumTiposDeLog.TiposDeLog.Erro);
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

        /// <summary>
        /// Método que faz a limpeza dos arquivos de log com mais de uma semana de existencia.
        /// </summary>
        private void EfetuarLimpezaDeArquivos(EnumTiposDeLog.TiposDeLog tipoDoLog = EnumTiposDeLog.TiposDeLog.Informacao)
        {
            try
            {
                string caminhoLog = "";
                int periodoArmazenagem = 0;
                string nomeDoAplicativo;

                if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["Logger.PeriodoDeArmazenagem"]))
                {
                    periodoArmazenagem = Convert.ToInt32(WebConfigurationManager.AppSettings["Logger.PeriodoDeArmazenagem"]);
                }
                else
                {
                    periodoArmazenagem = 7;
                }

                if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["Logger.NomeDaAplicacao"]))
                {
                    nomeDoAplicativo = WebConfigurationManager.AppSettings["Logger.NomeDaAplicacao"].ToString();
                }
                else
                {
                    nomeDoAplicativo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                }

                if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["Logger.DiretorioDeGravacao"]))
                {
                    string nomeDoArquivo = "Logger_" + tipoDoLog + "_" + nomeDoAplicativo + "_" + DateTime.Today.AddDays(-periodoArmazenagem).ToString("dd-MM-yyyy") + ".json";
                    caminhoLog = WebConfigurationManager.AppSettings["Logger.DiretorioDeGravacao"].ToString() + nomeDoArquivo;
                }
                else
                {
                    string nomeDoArquivo = "Logger_" + tipoDoLog + "_" + nomeDoAplicativo + "_" + DateTime.Today.AddDays(-periodoArmazenagem).ToString("dd-MM-yyyy") + ".json";
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
        private void PreencherLog<T>(T DadosParaLog, EnumTiposDeLog.TiposDeLog tipoDeLog = EnumTiposDeLog.TiposDeLog.Informacao)
        {
            try
            {
                string Dadosjson = JsonConvert.SerializeObject(new { InformacoesLog = DadosParaLog, TipoDoLog = tipoDeLog.ToString(), DataDoLog = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }, Formatting.Indented,
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
        private void PreencherLog<T>(T dadosLog, Dictionary<object, object> DicionarioDadosLog, EnumTiposDeLog.TiposDeLog tipoDeLog = EnumTiposDeLog.TiposDeLog.Informacao)
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
        private void PreencherLog<T>(T DadosParaLog, dynamic InformacoesAdicionais, EnumTiposDeLog.TiposDeLog tipoDeLog = EnumTiposDeLog.TiposDeLog.Informacao)
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