using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsultorioMedico
{
    public class Pessoa
    {
        private string nome;
        public string Nome 
        { 
            get { return nome; } 
            set { nome = value; } 
        }
    }
    public class Medico : Pessoa
    {
        private string crm;
        private string especialidade;
        
        public string CRM 
        { 
            get { return crm; } 
            set { crm = value; } 
        }
        
        public string Especialidade 
        { 
            get { return especialidade; } 
            set { especialidade = value; } 
        }
    }
    public class Paciente : Pessoa
    {
        private string cpf;
        private DateTime dataNascimento;
        
        public string CPF 
        { 
            get { return cpf; } 
            set { cpf = value; } 
        }
        
        public DateTime DataNascimento 
        { 
            get { return dataNascimento; } 
            set { dataNascimento = value; } 
        }
    }
    public class Consulta
    {
        private string crm;
        private string cpf;
        private DateTime dataHora;
        
        public string CRM 
        { 
            get { return crm; } 
            set { crm = value; } 
        }
        
        public string CPF 
        { 
            get { return cpf; } 
            set { cpf = value; } 
        }
        
        public DateTime DataHora 
        { 
            get { return dataHora; } 
            set { dataHora = value; } 
        }
    }
    public class ClinicaService
    {
        public List<Medico> Medicos { get; set; } = new();
        public List<Paciente> Pacientes { get; set; } = new();
        public List<Consulta> Consultas { get; set; } = new();

        public bool HorarioDisponivel(string crm, DateTime dataHora)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(crm))
                    throw new ArgumentException("CRM não pode ser nulo ou vazio.");

                if (Consultas == null)
                    throw new InvalidOperationException("Lista de consultas não foi inicializada.");

                return !Consultas.Any(c => c.CRM == crm && c.DataHora == dataHora);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao verificar disponibilidade de horário: {ex.Message}", ex);
            }
        }

        public string CadastrarMedico(Medico medico)
        {
            try
            {
                if (medico == null)
                    return "Médico não pode ser nulo.";

                if (string.IsNullOrWhiteSpace(medico.Nome))
                    return "Nome do médico é obrigatório.";

                if (string.IsNullOrWhiteSpace(medico.CRM))
                    return "CRM é obrigatório.";

                if (string.IsNullOrWhiteSpace(medico.Especialidade))
                    return "Especialidade é obrigatória.";

                if (Medicos.Any(m => m.CRM == medico.CRM))
                    return "Já existe um médico cadastrado com este CRM.";

                Medicos.Add(medico);
                return "Médico cadastrado com sucesso!";
            }
            catch (ArgumentNullException ex)
            {
                return $"Erro de argumento nulo: {ex.Message}";
            }
            catch (InvalidOperationException ex)
            {
                return $"Operação inválida: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Erro inesperado ao cadastrar médico: {ex.Message}";
            }
        }

        public string CadastrarPaciente(Paciente paciente)
        {
            try
            {
                if (paciente == null)
                    return "Paciente não pode ser nulo.";

                if (string.IsNullOrWhiteSpace(paciente.Nome))
                    return "Nome do paciente é obrigatório.";

                if (string.IsNullOrWhiteSpace(paciente.CPF))
                    return "CPF é obrigatório.";

                if (paciente.DataNascimento == default(DateTime) || paciente.DataNascimento > DateTime.Now)
                    return "Data de nascimento inválida.";

                if (Pacientes.Any(p => p.CPF == paciente.CPF))
                    return "Já existe um paciente cadastrado com este CPF.";

                Pacientes.Add(paciente);
                return "Paciente cadastrado com sucesso!";
            }
            catch (ArgumentNullException ex)
            {
                return $"Erro de argumento nulo: {ex.Message}";
            }
            catch (InvalidOperationException ex)
            {
                return $"Operação inválida: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Erro inesperado ao cadastrar paciente: {ex.Message}";
            }
        }

        public string AgendarConsulta(Consulta consulta)
        {
            try
            {
                if (consulta == null)
                    return "Consulta inválida.";

                if (string.IsNullOrWhiteSpace(consulta.CRM))
                    return "CRM é obrigatório para agendar consulta.";

                if (string.IsNullOrWhiteSpace(consulta.CPF))
                    return "CPF é obrigatório para agendar consulta.";

                if (consulta.DataHora == default(DateTime))
                    return "Data e hora da consulta são obrigatórias.";

                if (consulta.DataHora < DateTime.Now)
                    return "A consulta não pode ser marcada no passado.";

                if (Medicos == null || Pacientes == null || Consultas == null)
                    return "Sistema não foi inicializado corretamente.";

                var medico = Medicos.FirstOrDefault(m => m.CRM == consulta.CRM);
                var paciente = Pacientes.FirstOrDefault(p => p.CPF == consulta.CPF);

                if (medico == null)
                    return "Médico não encontrado. Verifique se o CRM está correto.";

                if (paciente == null)
                    return "Paciente não encontrado. Verifique se o CPF está correto.";

                int consultasDoDia = Consultas.Count(c => c.CRM == consulta.CRM && c.DataHora.Date == consulta.DataHora.Date);
                if (consultasDoDia >= 10)
                    return "Este médico já tem 10 consultas nesse dia.";

                bool pacienteJaAgendado = Consultas.Any(c =>
                    c.CRM == consulta.CRM &&
                    c.CPF == consulta.CPF &&
                    c.DataHora.Date == consulta.DataHora.Date);

                if (pacienteJaAgendado)
                    return "Este paciente já tem uma consulta com esse médico nesse dia.";

                if (!HorarioDisponivel(consulta.CRM, consulta.DataHora))
                    return "Horário indisponível para esse médico.";

                Consultas.Add(consulta);
                return "Consulta marcada com sucesso!";
            }
            catch (ArgumentNullException ex)
            {
                return $"Erro de argumento nulo: {ex.Message}";
            }
            catch (ArgumentException ex)
            {
                return $"Argumento inválido: {ex.Message}";
            }
            catch (InvalidOperationException ex)
            {
                return $"Operação inválida: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Erro inesperado ao agendar consulta: {ex.Message}";
            }
        }

        public string CancelarConsulta(string crm, string cpf, DateTime dataHora)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(crm))
                    return "CRM é obrigatório para cancelar consulta.";

                if (string.IsNullOrWhiteSpace(cpf))
                    return "CPF é obrigatório para cancelar consulta.";

                if (dataHora == default(DateTime))
                    return "Data e hora são obrigatórias para cancelar consulta.";

                var consulta = Consultas.FirstOrDefault(c => 
                    c.CRM == crm && 
                    c.CPF == cpf && 
                    c.DataHora == dataHora);

                if (consulta == null)
                    return "Consulta não encontrada.";

                if (consulta.DataHora <= DateTime.Now.AddHours(24))
                    return "Consulta só pode ser cancelada com pelo menos 24 horas de antecedência.";

                Consultas.Remove(consulta);
                return "Consulta cancelada com sucesso!";
            }
            catch (ArgumentNullException ex)
            {
                return $"Erro de argumento nulo: {ex.Message}";
            }
            catch (InvalidOperationException ex)
            {
                return $"Operação inválida: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Erro inesperado ao cancelar consulta: {ex.Message}";
            }
        }
    }
   
    internal class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
