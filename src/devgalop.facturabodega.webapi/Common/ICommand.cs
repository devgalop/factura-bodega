using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Common;

public interface ICommand{}

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    /// <summary>
    /// Captura el comando y ejecuta la lógica asociada
    /// </summary>
    /// <param name="command">Comando a ejecutar</param>
    /// <returns></returns>
    Task HandleAsync(TCommand command);
}