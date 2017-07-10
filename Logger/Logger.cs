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
        private static string caminhoDoLogEstaticoAlerta = string.Empty;
        private static string caminhoDoLogEstaticoErro = string.Empty;


        private static Dictionary<Guid, string> linhasEscreverAlerta = new Dictionary<Guid, string>();
        private static Dictionary<Guid, string> linhasEscreverErro = new Dictionary<Guid, string>();
        private static Dictionary<Guid, string> linhasEscreverInformacao = new Dictionary<Guid, string>();

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

            if (!loggerAtivo)
            {
                EventLog eventoLog = new EventLog();
                eventoLog.Source = "LOGGER - Sistema de Log";
                eventoLog.WriteEntry("O LOGGER não esta ativido no momento para LOG", EventLogEntryType.Information);
            }
            else
            {
                if (!Logger.escrevendoLog)
                {
                    Logger.escrevendoLog = true;
                    CriarArquivoFisicoLog();
                    CriarArquivoFisicoLog(EnumTiposDeLog.TiposDeLog.Alerta);
                    CriarArquivoFisicoLog(EnumTiposDeLog.TiposDeLog.Erro);
                    Logger.LogWriterAsync();
                }
            }
        }

        //private static void EscreverLog()
        //{
        //    var tr = new Task<bool>(() => IniciarEscreverLog());
        //    tr.Start();

        //}

        //private static bool IniciarEscreverLog(Dictionary<Guid, string> dadosLog)
        //{
        //    try
        //    {

        //        var informacoesLog = dadosLog.ToList();

        //        for (int i = 0; i < informacoesLog.Count; i++)
        //        {
        //            File.AppendAllText(caminhoDoLogEstatico, informacoesLog[i].Value + Environment.NewLine);
        //        }

        //        //linhasEscrever.Remove(item.Key);
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLog eventoLog = new EventLog();
        //        eventoLog.Source = "LOGGER - Sistema de Log";
        //        eventoLog.WriteEntry("O método IniciarEscreverLog() - Escrever um registro - falhou. Mensagem da exception: " + ex.Message + "/// Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
        //    }

        //    return true;
        //}

        private static void LogWriterAsync()
        {
            //var tr = new Task<bool>(() => IniciarEscreverLog());
            var tr = new Thread(() => IniciarEscreverLog());
            tr.IsBackground = true;
            tr.Start();           
           
        }


        private static bool IniciarEscreverLog()
        {
            try
            {
                for (long i = 0; i <= Int64.MaxValue; i++)
                {
                    Thread.Sleep(5000);

                    if (linhasEscreverInformacao.Count > 0)
                    {
                        var linhasAtual = linhasEscreverInformacao.Take(1000).ToList();

                        foreach (var item in linhasAtual)
                        {
                            try
                            {
                                File.AppendAllText(caminhoDoLogEstatico, item.Value + Environment.NewLine);
                                linhasEscreverInformacao.Remove(item.Key);
                            }
                            catch (Exception ex)
                            {
                                EventLog eventoLog = new EventLog();
                                eventoLog.Source = "LOGGER - Sistema de Log";
                                eventoLog.WriteEntry("O método IniciarEscreverLog() - Escrever um registro - falhou. Mensagem da exception: " + ex.Message + "/// Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
                            }
                        }
                    }

                    if (linhasEscreverErro.Count > 0)
                    {
                        var linhasAtual = linhasEscreverErro.Take(1000).ToList();

                        foreach (var item in linhasAtual)
                        {
                            try
                            {
                                File.AppendAllText(caminhoDoLogEstaticoErro, item.Value + Environment.NewLine);
                                linhasEscreverErro.Remove(item.Key);
                            }
                            catch (Exception ex)
                            {
                                EventLog eventoLog = new EventLog();
                                eventoLog.Source = "LOGGER - Sistema de Log";
                                eventoLog.WriteEntry("O método IniciarEscreverLog() - Escrever um registro - falhou. Mensagem da exception: " + ex.Message + "/// Stacktrace da exception: " + ex.StackTrace, EventLogEntryType.Error);
                            }
                        }
                    }


                    if (linhasEscreverAlerta.Count > 0)
                    {
                        var linhasAtual = linhasEscreverAlerta.Take(1000).ToList();

                        foreach (var item in linhasAtual)
                        {
                            try
                            {
                                File.AppendAllText(caminhoDoLogEstaticoAlerta, item.Value + Environment.NewLine);
                                linhasEscreverAlerta.Remove(item.Key);
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
                if (loggerAtivo)
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
                if (loggerAtivo)
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
        public void FazerLogAsync<T>(T DadosLog, dynamic SegundoDados, EnumTiposDeLog.TiposDeLog tipoDoLog = EnumTiposDeLog.TiposDeLog.Informacao)
        {
            var tr = new Thread(() => FazerLog(DadosLog, SegundoDados, tipoDoLog));           
            tr.Start();
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
                if (loggerAtivo)
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
                return false;   
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
                    string nomeDoArquivo = "Logger_" + tipoDoLog + "_" + nomeDoAplicativo + "_" + DateTime.Today.ToString("dd-MM-yyyy") + ".json";
                    caminhoDoLog = WebConfigurationManager.AppSettings["Logger.DiretorioDeGravacao"].ToString() + nomeDoArquivo;

                    if (tipoDoLog == EnumTiposDeLog.TiposDeLog.Informacao)
                    {
                        Logger.caminhoDoLogEstatico = caminhoDoLog;
                    }
                    else if (tipoDoLog == EnumTiposDeLog.TiposDeLog.Erro)
                    {
                        Logger.caminhoDoLogEstaticoErro = caminhoDoLog;
                    }
                    else if (tipoDoLog == EnumTiposDeLog.TiposDeLog.Alerta)
                    {
                        Logger.caminhoDoLogEstaticoAlerta = caminhoDoLog;
                    }
                }
                else
                {
                    string nomeDoArquivo = "Logger_" + tipoDoLog + "_" + nomeDoAplicativo + "_" + DateTime.Today.ToString("dd-MM-yyyy") + ".json";
                    caminhoDoLog = System.AppDomain.CurrentDomain.BaseDirectory + "Logger\\";

                    if (tipoDoLog == EnumTiposDeLog.TiposDeLog.Informacao)
                    {
                        Logger.caminhoDoLogEstatico = caminhoDoLog;
                    }
                    else if (tipoDoLog == EnumTiposDeLog.TiposDeLog.Erro)
                    {
                        Logger.caminhoDoLogEstaticoErro = caminhoDoLog;
                    }
                    else if (tipoDoLog == EnumTiposDeLog.TiposDeLog.Alerta)
                    {
                        Logger.caminhoDoLogEstaticoAlerta = caminhoDoLog;
                    }

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
                    EfetuarLimpezaDeArquivos();
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
        private void EfetuarLimpezaDeArquivos()
        {
            try
            {
                string caminhoLogInterno = "";
                int periodoArmazenagem = 0;
                string nomeDoAplicativo;
                DateTime periodoDeExclusao;
                long tamanhoLimiteDoArquivo;

                if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["Logger.PeriodoDeArmazenagem"]))
                {
                    periodoArmazenagem = Convert.ToInt32(WebConfigurationManager.AppSettings["Logger.PeriodoDeArmazenagem"]);
                    periodoDeExclusao = DateTime.Today.AddDays(-periodoArmazenagem);
                }
                else
                {
                    periodoArmazenagem = 7;
                    periodoDeExclusao = DateTime.Today.AddDays(-periodoArmazenagem);
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
                    caminhoLogInterno = WebConfigurationManager.AppSettings["Logger.DiretorioDeGravacao"].ToString();
                    if (!caminhoLogInterno.EndsWith("\\"))
                    {
                        caminhoLogInterno = caminhoLogInterno + "\\";
                    }
                }
                else
                {
                    caminhoLogInterno = System.AppDomain.CurrentDomain.BaseDirectory + "Logger\\";
                    if (!caminhoLogInterno.EndsWith("\\"))
                    {
                        caminhoLogInterno = caminhoLogInterno + "\\";
                    }
                }

                if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["Logger.TamanhoLimiteDoArquivo"]))
                {
                    tamanhoLimiteDoArquivo = Convert.ToInt64(WebConfigurationManager.AppSettings["Logger.TamanhoLimiteDoArquivo"]);
                }
                else
                {
                    tamanhoLimiteDoArquivo = Convert.ToInt64(1073741824);
                }

                List<string> ArquivosEncontrados = Directory.GetFiles(caminhoLogInterno, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".json")).ToList();

                for (int i = 0; i < ArquivosEncontrados.Count; i++)
                {
                    DateTime dataDeCriacaoDoArquivo = File.GetCreationTime(ArquivosEncontrados[i]);
                    FileInfo arquivoAtual = new FileInfo(ArquivosEncontrados[i]);

                    if (dataDeCriacaoDoArquivo < periodoDeExclusao)
                    {
                        File.Delete(ArquivosEncontrados[i].ToString());
                    }
                    else if (arquivoAtual.Length > tamanhoLimiteDoArquivo)
                    {
                        File.Delete(ArquivosEncontrados[i].ToString());
                    }
                }
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
            Dictionary<Guid, string> dadosParaEscreverLog = new Dictionary<Guid, string>();
            try
            {
                string Dadosjson = JsonConvert.SerializeObject(new { InformacoesLog = DadosParaLog, TipoDoLog = tipoDeLog.ToString(), DataDoLog = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Include
                    });

                if (tipoDeLog == EnumTiposDeLog.TiposDeLog.Informacao)
                {
                    Logger.linhasEscreverInformacao.Add(Guid.NewGuid(), Dadosjson);
                }
                else if (tipoDeLog == EnumTiposDeLog.TiposDeLog.Erro)
                {
                    Logger.linhasEscreverErro.Add(Guid.NewGuid(), Dadosjson);
                }
                else if (tipoDeLog == EnumTiposDeLog.TiposDeLog.Alerta)
                {
                    Logger.linhasEscreverAlerta.Add(Guid.NewGuid(), Dadosjson);
                }
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
            Dictionary<Guid, string> dadosParaEscreverLog = new Dictionary<Guid, string>();
            try
            {
                string Dadosjson = JsonConvert.SerializeObject(new { InformacoesPrincipaisLog = dadosLog, InformacoesAdicionais = DicionarioDadosLog, DataDoLog = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Include
                    });

                if (tipoDeLog == EnumTiposDeLog.TiposDeLog.Informacao)
                {
                    Logger.linhasEscreverInformacao.Add(Guid.NewGuid(), Dadosjson);
                }
                else if (tipoDeLog == EnumTiposDeLog.TiposDeLog.Erro)
                {
                    Logger.linhasEscreverErro.Add(Guid.NewGuid(), Dadosjson);
                }
                else if (tipoDeLog == EnumTiposDeLog.TiposDeLog.Alerta)
                {
                    Logger.linhasEscreverAlerta.Add(Guid.NewGuid(), Dadosjson);
                }
                
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
            Dictionary<Guid, string> dadosParaEscreverLog = new Dictionary<Guid, string>();
            try
            {
                string Dadosjson = JsonConvert.SerializeObject(new { InformacoesLog = DadosParaLog, InformacoesAdicionais, DataDoLog = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    });

                if (tipoDeLog == EnumTiposDeLog.TiposDeLog.Informacao)
                {
                    Logger.linhasEscreverInformacao.Add(Guid.NewGuid(), Dadosjson);
                }
                else if (tipoDeLog == EnumTiposDeLog.TiposDeLog.Erro)
                {
                    Logger.linhasEscreverErro.Add(Guid.NewGuid(), Dadosjson);
                }
                else if (tipoDeLog == EnumTiposDeLog.TiposDeLog.Alerta)
                {
                    Logger.linhasEscreverAlerta.Add(Guid.NewGuid(), Dadosjson);
                }
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