using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Mettle
{
    #pragma warning disable S4457

    public partial class AssertImpl
    {
        /// <summary>
        /// Verifies that the provided object raised <see cref="INotifyPropertyChanged.PropertyChanged"/>
        /// as a result of executing the given test code.
        /// </summary>
        /// <param name="object">The object which should raise the notification.</param>
        /// <param name="propertyName">The property name for which the notification should be raised.</param>
        /// <param name="testCode">The test code which should cause the notification to be raised.</param>
        /// <returns>An instance of <see cref="IAssert" />.</returns>
        /// <exception cref="PropertyChangedException">Thrown when the notification is not raised.</exception>
        public IAssert PropertyChanged(INotifyPropertyChanged @object, string propertyName, Action testCode)
        {
            if (@object is null)
                throw new ArgumentNullException(nameof(@object));

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (testCode is null)
                throw new ArgumentNullException(nameof(testCode));

            var propertyChangeHappened = false;

            PropertyChangedEventHandler handler = (sender, args) => propertyChangeHappened |= string.IsNullOrEmpty(args.PropertyName) || propertyName.Equals(args.PropertyName, StringComparison.OrdinalIgnoreCase);

            @object.PropertyChanged += handler;

            try
            {
                testCode();
                if (!propertyChangeHappened)
                    throw new PropertyChangedException(propertyName);
            }
            finally
            {
                @object.PropertyChanged -= handler;
            }

            return this;
        }

        /// <summary>
        /// Verifies that the provided object raised <see cref="INotifyPropertyChanged.PropertyChanged"/>
        /// as a result of executing the given test code.
        /// </summary>
        /// <param name="object">The object which should raise the notification.</param>
        /// <param name="propertyName">The property name for which the notification should be raised.</param>
        /// <param name="testCode">The test code which should cause the notification to be raised.</param>
        /// <returns>An instance of <see cref="Task" />.</returns>
        /// <exception cref="PropertyChangedException">Thrown when the notification is not raised.</exception>
        public async Task PropertyChangedAsync(INotifyPropertyChanged @object, string propertyName, Func<Task> testCode)
        {
            if (@object is null)
                throw new ArgumentNullException(nameof(@object));

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (testCode is null)
                throw new ArgumentNullException(nameof(testCode));

            var propertyChangeHappened = false;

            PropertyChangedEventHandler handler = (sender, args) => propertyChangeHappened |= string.IsNullOrEmpty(args.PropertyName) || propertyName.Equals(args.PropertyName, StringComparison.OrdinalIgnoreCase);

            @object.PropertyChanged += handler;

            try
            {
                await testCode();
                if (!propertyChangeHappened)
                    throw new PropertyChangedException(propertyName);
            }
            finally
            {
                @object.PropertyChanged -= handler;
            }
        }
    }
}