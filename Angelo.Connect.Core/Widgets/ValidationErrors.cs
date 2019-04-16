using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Widgets
{
    public class ValidationErrors
    {
        private IDictionary<string, List<string>> _fieldErrors;
        private int _errorCount = 0;

        public int ErrorCount { get { return _errorCount; } }
        public bool HasErrors { get { return _errorCount > 0; } }

        public ValidationErrors()
        {
            _fieldErrors = new Dictionary<string, List<string>>();
        }

        public ValidationErrors(ICollection<ValidationResult> validationResults) : this()
        {
            if (validationResults != null)
                BuildErrorDictionary(validationResults);
        }

        public void BuildErrorDictionary(ICollection<ValidationResult> validationResults)
        {
            foreach (var result in validationResults)
            {
                foreach (var field in result.MemberNames)
                {
                    if (!_fieldErrors.ContainsKey(field))
                        _fieldErrors.Add(field, new List<string>());

                    _fieldErrors[field].Add(result.ErrorMessage);
                    _errorCount++;
                }
            }
        }

        /// <summary>
        /// Returns all errors for this control's model
        /// </summary>
        public IEnumerable<string> GetErrors()
        {
            return _fieldErrors.SelectMany(x => x.Value) ?? new List<string>();
        }

        /// <summary>
        /// Returns errors for the specified field in this control's model
        /// </summary>
        /// <param name="fieldName">model field</param>
        public IEnumerable<string> GetErrors(string fieldName)
        {
            return _fieldErrors.ContainsKey(fieldName) ? _fieldErrors[fieldName] : new List<string>();
        }

    }
}
