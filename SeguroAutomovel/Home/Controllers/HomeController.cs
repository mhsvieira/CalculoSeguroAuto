using SeguroAutomovel.Home.DataAccess;
using SeguroAutomovel.Home.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SeguroAutomovel.Home.Controllers
{
    [Authorize(Roles = "Admin,User")]
    [RoutePrefix("api/seguroauto/v1")]
    public class HomeController : ApiController
    {
        [HttpGet]
        [Route("pesquisar/id/{id:int}")]
        // api/seguroauto/v1/pesquisar/id/Id
        public HttpResponseMessage PesquisarSeguroAuto(int id)
        {
            if (id <= 0)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Id não informado");

            try
            {
                OperacoesBD operacao = new OperacoesBD();

                Apolice apolice = operacao.PesquisarSeguroAuto(id);

                if (apolice.Id > 0)
                    return Request.CreateResponse(HttpStatusCode.OK, apolice);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Falha ao pesquisar apólice");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message + " - Erro método PesquisarSeguroAuto");
            }
        }

        [HttpGet]
        [Route("pesquisar/cpf/{cpf}")]
        // api/seguroauto/v1/pesquisar/cpf/cpf
        public HttpResponseMessage PesquisarSeguroAuto(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Cpf não informado");

            try
            {

                cpf = cpf.PadLeft(11, '0');

                cpf = cpf.Substring(0, 3) + "." + cpf.Substring(3, 3) + "." + cpf.Substring(6, 3) + "-" + cpf.Substring(9, 2);

                OperacoesBD operacao = new OperacoesBD();

                List<Apolice> listApolices = operacao.PesquisarSeguroAuto(cpf);

                if (listApolices.Count > 0)
                    return Request.CreateResponse(HttpStatusCode.OK, listApolices);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Falha ao pesquisar apólice");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message + " - Erro método PesquisarSeguroAuto");
            }
        }

        [HttpGet]
        [Route("pesquisar/data/{datainicio}/{datafinal}")]
        // api/seguroauto/v1/pesquisar/data/datainicio/datafinal
        public HttpResponseMessage PesquisarSeguroAuto(DateTime dataInicio, DateTime datafinal)
        {
            if (datafinal < dataInicio)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Informe o período corretamente");

            try
            {
                OperacoesBD operacao = new OperacoesBD();

                List<Apolice> listApolices = operacao.PesquisarSeguroAuto(dataInicio, datafinal);

                if (listApolices.Count > 0)
                    return Request.CreateResponse(HttpStatusCode.OK, listApolices);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Falha ao pesquisar apólices");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message + " - Erro método PesquisarSeguroAuto");
            }
        }

        [HttpGet]
        [Route("relatorio/{datainicio}/{datafinal}")]
        // api/seguroauto/v1/relatorio/datainicio/datafinal
        public HttpResponseMessage RelatorioSeguroAuto(DateTime dataInicio, DateTime datafinal)
        {
            if (datafinal < dataInicio)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Informe o período corretamente");

            try
            {
                OperacoesBD operacao = new OperacoesBD();

                Relatorio relatorio = operacao.RelatorioMediasSeguroAuto(dataInicio, datafinal);

                if (relatorio.QtdRegistros > 0)
                    return Request.CreateResponse(HttpStatusCode.OK, relatorio);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Falha ao extrair relatório");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message + " - Erro método RelatorioSeguroAuto");
            }
        }

        [HttpPost]
        [Route("calcular")]
        // api/seguroauto/v1/calcular
        public HttpResponseMessage CalcularSeguroAuto([FromBody] Documento documento)
        {
            if (string.IsNullOrEmpty(documento.Cpf))
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "CPF não informado");

            try
            {
                OperacoesBD operacao = new OperacoesBD();

                Apolice calculo = operacao.calcularSeguroAuto(documento.Cpf);

                if (calculo.PremioTotal > 0)
                {
                    Apolice apolice = operacao.gravarSeguroAuto(calculo);

                    if (apolice.Id > 0)
                        return Request.CreateResponse(HttpStatusCode.OK, apolice);
                }

                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Falha ao calcular/gravar apólice");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message + " - Erro método calcularSeguroAuto");
            }
        }
    }
}