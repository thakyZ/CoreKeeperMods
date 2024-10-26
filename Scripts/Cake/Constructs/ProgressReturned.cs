using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Cli;
using Cake.Core.Diagnostics;
using NuGet.Protocol.Plugins;

namespace NekoBoiNick.CoreKeeperMods.Scripts.Cake.Constructs;
#region ProgressReturnedCollection{T}
public sealed class ProgressReturnedCollection<T> : IDisposable where T : notnull {
  /// <summary>
  /// Private instance of the collection of entries of the tasks.
  /// </summary>
  private List<IProgressReturned<T>> _entries = [];
  /// <summary>
  /// Collection of entries of the tasks.
  /// </summary>
  public IReadOnlyList<IProgressReturned<T>> Entries => _entries;
  /// <summary>
  /// The template for formatting the failed items in strings.
  /// </summary>
  private readonly string _failedItemMessageTemplate;
  /// <summary>
  /// The instance of a <see cref="ICakeLog" /> to print to the console.
  /// </summary>
  private readonly ICakeLog _logger;

  /// <summary>
  /// Constructs a new instance of <see cref="ProgressReturnedCollection{T} " />
  /// </summary>
  /// <param name="failedItemMessageTemplate">The template for formatting the failed items in strings.</param>
  /// <param name="logger">The instance of a <see cref="ICakeLog" /> to print to the console.</param>
  public ProgressReturnedCollection(string failedItemMessageTemplate, ICakeLog logger) {
    _failedItemMessageTemplate = failedItemMessageTemplate;
    _logger = logger;
  }

  /// <summary>
  /// Prints messages from the various entries in the Task completion.
  /// </summary>
  /// <param name="level">The level of which to log at (defaults to <see cref="LogLevel.Information" />.</param>
  /// <param name="verbosity">The level of verbosity of which to log at (defaults to <see cref="Verbosity.Normal" />.</param>
  public void PrintMessages(LogLevel level = LogLevel.Information, Verbosity verbosity = Verbosity.Normal) {
    foreach (var message in _entries.SelectMany(entry => entry.Messages)) {
      _logger.Write(verbosity, level, message);
    }
  }

  public void PrintExceptions(LogLevel level = LogLevel.Information, Verbosity verbosity = Verbosity.Normal) {
    foreach (var exception in _entries.Select(entry => entry.Exception)) {
      _logger.LogException(exception);
    }
  }

  public void PrintFailed(LogLevel level = LogLevel.Error, Verbosity verbosity = Verbosity.Normal) {
    foreach (var entry in _entries.Where(x => x.FailedItem is not null)) {
      _logger.Write(verbosity, level, _failedItemMessageTemplate, entry.FailedItem);
    }
  }

  public IProgressReturned<T> AddEntry(T item) {
    IProgressReturned<T> entry;
    if(item is IDisposable) {
      Type t = typeof(ProgressReturnedDisposable<>);
      t = t.MakeGenericType(typeof(T));
      var constructor = t.GetConstructors()[0];
      entry = (IProgressReturned<T>)constructor.Invoke([item]);
    } else {
      entry = new ProgressReturned<T>(item);
    }
    _entries.Add(entry);
    return entry;
  }

  public void Dispose() {
    foreach (IProgressReturned<T> entry in _entries) {
      (entry as IDisposable)?.Dispose();
    }
  }
}
#endregion

#region IProgressReturned{T}
public interface IProgressReturned<T> where T : notnull {
  /// <summary>
  /// List of messages to print at the end.
  /// </summary>
  public IReadOnlyList<string> Messages { get; }

  /// <summary>
  /// Exception when the task has failed.
  /// </summary>
  public Exception? Exception { get; }

  /// <summary>
  /// Determines if this task has failed.
  /// </summary>
  public bool Failed { get; }

  /// <summary>
  /// Shows the task item when failed.
  /// </summary>
  public T? FailedItem { get; }

  public IProgressReturned<T> SetException(Exception exception);

  public IProgressReturned<T> SetFailed();

  public IProgressReturned<T> AddMessage(string message);

  public IProgressReturned<T> AddMessage(string format, params object?[] args);
}
#endregion

#region ProgressReturnedBase{T}
public abstract class ProgressReturnedBase<T> : IProgressReturned<T> where T : notnull {
  /// <summary>
  /// Private instance of the message collection.
  /// </summary>
  protected internal bool isDisposable;

  /// <summary>
  /// Private instance of the message collection.
  /// </summary>
  private readonly List<string> _messages = [];

  /// <summary>
  /// Private instance of the item to store.
  /// </summary>
  protected internal readonly T _item;

  /// <summary>
  /// List of messages to print at the end.
  /// </summary>
  public IReadOnlyList<string> Messages => _messages;

  /// <summary>
  /// Exception when the task has failed.
  /// </summary>
  public Exception? Exception { get; private set; }

  /// <summary>
  /// Determines if this task has failed.
  /// </summary>
  public bool Failed { get; private set; }

  /// <summary>
  /// Shows the task item when failed.
  /// </summary>
  public T? FailedItem => Failed ? _item : default;

  /// <summary>
  /// Constructs a new instance of <see cref="ProgressReturned{T}"/>
  /// </summary>
  /// <param name="item">The item to cache and print upon failure.</param>
  protected ProgressReturnedBase(T item) {
    _item = item;
  }

  public IProgressReturned<T> SetException(Exception exception) {
    Exception = exception;
    Failed    = true;
    return this;
  }

  public IProgressReturned<T> SetFailed() {
    Failed = true;
    return this;
  }

  public IProgressReturned<T> AddMessage(string message) {
    _messages.Add(message);
    return this;
  }

  public IProgressReturned<T> AddMessage(string format, params object?[] args) {
    _messages.Add(string.Format(format, args));
    return this;
  }
}
#endregion

#region ProgressReturnedDisposable{T}
public sealed class ProgressReturnedDisposable<T> : ProgressReturnedBase<T>, IDisposable where T : notnull, IDisposable {
  public ProgressReturnedDisposable(T item) : base(item) {
    isDisposable = true;
  }

  internal static ProgressReturnedDisposable<T> Create(T item) {
    return new ProgressReturnedDisposable<T>(item);
  }

  public void Dispose() {
    _item.Dispose();
  }
}
#endregion

#region ProgressReturned{T}
public sealed class ProgressReturned<T> : ProgressReturnedBase<T> where T : notnull {
  /// <summary>
  /// Constructs a new instance of <see cref="ProgressReturned{T}"/>
  /// </summary>
  /// <param name="item">The item to cache and print upon failure.</param>
  public ProgressReturned(T item) : base(item) { }
}
#endregion
