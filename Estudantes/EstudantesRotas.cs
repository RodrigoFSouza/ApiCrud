using ApiCrud.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiCrud.Estudantes;

public static class EstudantesRotas
{
    public static void AddRotasEstudantes(this WebApplication app)
    {
        // CancellationToken é usado em consultas assincronas em caso de processos demorados se a aplicação for encerrada
        // o banco também vai parar de executar o processo.
        app.MapPost("estudantes", async (AddEstudanteRequest request, AppDbContext context, CancellationToken ct) =>
        {
            var jaExiste = await context.Estudantes
                .AnyAsync(estudante => estudante.Nome == request.Nome);

            if (jaExiste)
                return Results.Conflict("Já Existe um estudante com esse nome.");
            
            var novoEstudante = new Estudante(request.Nome);
            await context.Estudantes.AddAsync(novoEstudante, ct);
            await context.SaveChangesAsync(ct);

            var estudanteRetorno = new EstudanteDto(novoEstudante.Id, novoEstudante.Nome);

            return Results.Ok(novoEstudante);
        });
        
        app.MapGet("estudantes", async (AppDbContext context, CancellationToken ct) =>
        {
            var estudantes = await context
                .Estudantes
                .Where(estudante => estudante.Ativo)
                .Select(estudante => new EstudanteDto(estudante.Id, estudante.Nome))
                .ToListAsync(ct);
                
            return estudantes;
        });

        app.MapPut("estudantes/{id:guid}", async (Guid id, UpdateEstudanteRequest request, AppDbContext context, CancellationToken ct) =>
        {
            var estudante = await context.Estudantes
                .SingleOrDefaultAsync(ct);

            if (estudante == null)
                return Results.NotFound();

            estudante.AtualizarNome(request.Nome);
            await context.SaveChangesAsync(ct);
            return Results.Ok(new EstudanteDto(estudante.Id, estudante.Nome));
        });

        app.MapDelete("estudante/{id:guid}", async (Guid id, AppDbContext context, CancellationToken ct) =>
        {
            var estudante = await context
                .Estudantes
                .SingleOrDefaultAsync(estudante => estudante.Id == id, ct);

            if (estudante == null)
                return Results.NotFound();
            
            estudante.Desativar();
            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        });

    }
}