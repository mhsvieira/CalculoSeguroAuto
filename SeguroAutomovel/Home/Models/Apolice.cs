using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SeguroAutomovel.Home.Models
{
    public class Apolice
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public DateTime DataSeguro { get; set; }
        public DateTime InicioVigencia { get; set; }
        public DateTime FinalVigencia { get; set; }
        public string CepPernoite { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public decimal ValorIS { get; set; }
        public decimal PremioTotal { get; set; }
        public string Status { get; set; }
    }
}