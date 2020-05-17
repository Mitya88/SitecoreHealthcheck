namespace Healthcheck.Service.LogParsing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;

	/// <summary>
	/// Log entry
	/// </summary>
	public class LogEntry
	{
		/// <summary>
		/// Get/Sets the Message of the LogEntry
		/// </summary>
		/// <value></value>
		public LogMessage Message { get; set; }

		/// <summary>
		/// Get/Sets the Time of the LogEntry
		/// </summary>
		/// <value></value>
		public DateTime Time { get; set; }

		/// <summary>
		/// Get/Sets the Level of the LogEntry
		/// </summary>
		/// <value></value>
		public LogLevel Level { get; set; }

		/// <summary>
		/// Get/Sets the Thread of the LogEntry
		/// </summary>
		/// <value></value>
		public string Thread { get; set; }

		/// <summary>
		/// Get/Sets the Logger of the LogEntry
		/// </summary>
		/// <value></value>
		public string Logger { get; set; }

		/// <summary>
		/// Gets or sets the process.
		/// </summary>
		/// <value>
		/// The process.
		/// </value>
		public string Process { get; set; }

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current <see cref="log4netParser.LogEntry"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current <see cref="log4netParser.LogEntry"/>.</returns>
		public override string ToString()
		{
			return Time.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + Level + "\t" + Logger + "\t" + Message;
		}
	}

	/// <summary>
	/// Log message
	/// </summary>
	/// <seealso cref="System.IComparable" />
	public class LogMessage : IComparable
	{
		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		public string Message { get; set; }

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared. 
		/// The return value has the following meanings: 
		/// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.
		/// Zero This object is equal to <paramref name="other"/>. 
		/// Greater than zero This object is greater than <paramref name="other"/>. 
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public int CompareTo(object other)
		{
			var logMessage = other as LogMessage;
			if (logMessage == null) return 1;
			if (logMessage.Message == null && Message == null) return 0;
			if (logMessage.Message == null) return 1;
			if (Message == null) return 0;
			//if (DuoVia.FuzzyStrings.StringExtensions.FuzzyEquals(Message, logMessage.Message)) return 0;
			return String.Compare(Message, logMessage.Message, StringComparison.Ordinal);
		}

		/// <summary>
		/// Initializes a new instance of the <b>LogMessage</b> class.
		/// </summary>
		/// <param name="message"></param>
		public LogMessage(string message)
		{
			Message = message;
		}

		/// <summary>
		/// Converts to string.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return Message;
		}

	}
}