// Models/GameChatMessage.cs
namespace TheInterrogatorAIDetective.Models
{
    /// <summary>
    /// Represents a single message in the conversation history between the player and an AI suspect.
    /// This class is used to structure the dialogue for display and for sending to the LLM.
    /// </summary>
    public class GameChatMessage
    {
        /// <summary>
        /// Gets or sets the role of the entity that produced this message.
        /// Expected values are "user" (for player messages) or "model" (for AI suspect responses).
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the textual content of the message.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameChatMessage"/> class.
        /// </summary>
        /// <param name="role">The role of the message sender (e.g., "user" or "model").</param>
        /// <param name="content">The content of the message.</param>
        /// <exception cref="ArgumentNullException">Thrown if role or content is null or whitespace.</exception>
        public GameChatMessage(string role, string content)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentNullException(nameof(role), "Role cannot be null or whitespace.");
            if (string.IsNullOrWhiteSpace(content)) // Allow empty content if intended, but typically content is expected.
                throw new ArgumentNullException(nameof(content), "Content cannot be null or whitespace for a meaningful message.");

            Role = role;
            Content = content;
        }
    }
}
