using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public abstract class BaseFactory<T> : IPaintCommandFactory where T : IInitializablePaintCommand, new()
    {
        private readonly IEditableImage2f _editableImage;
        private readonly IEditableMask2e _editableMask;
        private readonly IThreadSafeLogger _logger;

        protected BaseFactory(IEditableImage2f editableImage, IEditableMask2e editableMask, IThreadSafeLogger logger)
        {
            _editableImage = editableImage;
            _editableMask = editableMask;
            _logger = logger;
        }

        public abstract string Action { get; }

        public IPaintCommand Create()
        {
            var cmd = new T();
            cmd.Initialize(_editableImage, _editableMask, _logger);
            return cmd;
        }
    }
}