using Application.Core;
using System;

namespace Order.Domain.Services
{
    class ValidateNextState : IDomainService<List<string>, bool>
    {
        private static readonly Dictionary<string, List<string>> NextStateIsValid = new Dictionary<string, List<string>>
        {
            ["ToAssign"] = new List<string> { "ToAccept", "Cancelled" },
            ["ToAccept"] = new List<string> { "Accepted", "Cancelled" },
            ["Accepted"] = new List<string> { "Located", "Cancelled" },
            ["Located"] = new List<string> { "InProgress", "Cancelled" },
            ["InProgress"] = new List<string> { "Completed", "Cancelled" },
            ["Completed"] = new List<string> { "Paid", "Cancelled" },
            ["Paid"] = new List<string> { "Cancelled" },
            ["Cancelled"] = new List<string>()
        };

        public bool Execute(List<string> data)
        {
            string currentState = data[0];
            string nextState = data[1];

            if (NextStateIsValid.TryGetValue(currentState, out var validNextStates))
            {
                return validNextStates.Contains(nextState);
            }
            return false;
        }
    }
}
