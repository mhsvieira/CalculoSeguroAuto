using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeguroAutomovel.Home.DataAccess;
using SeguroAutomovel.Home.Models;

namespace SeguroAutomovelTest
{
    [TestClass]
    public class TesteUnitario1
    {

        [TestInitialize]
        public void InicializaTestes()
        {

        }

        [TestMethod]
        public void TesteCalculoeGravacaoSeguro()
        {
            OperacoesBD operacao = new OperacoesBD();

            Apolice calculo = operacao.calcularSeguroAuto("123.456.789-01");

            bool testaCalculo = calculo.PremioTotal > 0;

            Apolice apolice = operacao.gravarSeguroAuto(calculo);

            bool testaGravacao = apolice.Id > 0;

            bool resultadoTeste = testaCalculo && testaGravacao;

            Assert.AreEqual(true, resultadoTeste);
        }

        [TestCleanup]
        public void FinalizaTestes()
        {

        }
    }
}
