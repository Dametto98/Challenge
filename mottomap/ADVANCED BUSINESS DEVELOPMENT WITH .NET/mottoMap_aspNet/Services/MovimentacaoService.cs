using Microsoft.EntityFrameworkCore;
using MotoMap.Api.DotNet.Data;
using MotoMap.Api.DotNet.Dtos;
using MotoMap.Api.DotNet.Models;
using System; // Adicionado para DateTime

namespace MotoMap.Api.DotNet.Services
{
    public class MovimentacaoService : IMovimentacaoService
    {
        private readonly MotoMapDbContext _context;

        public MovimentacaoService(MotoMapDbContext context)
        {
            _context = context;
        }

        public async Task<Movimentacao> RegistrarEntradaAsync(MovimentacaoEntradaDto entradaDto)
        {
            // 1. Cria a nova movimentação (evento)
            var movimentacao = new Movimentacao
            {
                // CORREÇÃO: Removido .Value de MotoId (é int)
                MotoId = entradaDto.MotoId,
                // CORREÇÃO: Mantido .Value para PosicaoId (é int?)
                PosicaoId = entradaDto.PosicaoId.Value,
                UsuarioId = entradaDto.UsuarioId,
                Observacoes = entradaDto.Observacoes,
                Tipo = "ENTRADA", // Define o tipo
                DataHora = DateTime.UtcNow
            };
            _context.Movimentacoes.Add(movimentacao);

            // 2. Cria o novo histórico (estado)
            var historico = new HistoricoPosicao
            {
                // CORREÇÃO: Removido .Value de MotoId (é int)
                MotoId = entradaDto.MotoId,
                // CORREÇÃO: Mantido .Value para PosicaoId (é int?)
                PosicaoId = entradaDto.PosicaoId.Value,
                DataInicio = movimentacao.DataHora,
                DataFim = null // Nulo significa que está ocupado
            };
            _context.HistoricoPosicoes.Add(historico);

            // 3. Salva ambos no banco
            await _context.SaveChangesAsync();

            return movimentacao;
        }

        public async Task<HistoricoPosicao?> RegistrarSaidaAsync(MovimentacaoSaidaDto saidaDto)
        {
            // 1. Encontra o histórico "aberto" (DataFim == null) para esta moto
            var historicoAberto = await _context.HistoricoPosicoes
                // CORREÇÃO: Removido .Value de MotoId (é int)
                .FirstOrDefaultAsync(h => h.MotoId == saidaDto.MotoId && h.DataFim == null);

            if (historicoAberto == null)
            {
                return null; // Falha: Não há registro de entrada para esta moto
            }

            // 2. "Fecha" o histórico (estado)
            historicoAberto.DataFim = DateTime.UtcNow;
            _context.HistoricoPosicoes.Update(historicoAberto);

            // 3. Cria a movimentação de saída (evento)
            var movimentacao = new Movimentacao
            {
                // CORREÇÃO: Removido .Value de MotoId (é int)
                MotoId = saidaDto.MotoId,
                // CORREÇÃO (CS1061): Usando o PosicaoId do histórico que fechamos
                PosicaoId = historicoAberto.PosicaoId,
                UsuarioId = saidaDto.UsuarioId,
                Observacoes = saidaDto.Observacoes,
                Tipo = "SAIDA",
                DataHora = historicoAberto.DataFim.Value
            };
            _context.Movimentacoes.Add(movimentacao);

            // 4. Salva ambos no banco
            await _context.SaveChangesAsync();

            return historicoAberto;
        }
    }
}

