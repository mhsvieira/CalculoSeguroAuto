using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SeguroAutomovel.Home.Models
{
    public class Relatorio
    {
        public string Descricao { get; set; }
        public DateTime DataHoraExtracao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFinal { get; set; }
        public decimal MediaValorIS { get; set; }
        public decimal MediaPremioTotal { get; set; }
        public int QtdRegistros { get; set; }
    }
}