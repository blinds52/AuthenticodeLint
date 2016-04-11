﻿using AuthenticodeLint.Rules;
using System;

namespace AuthenticodeLint
{
    public class StdOutRuleResultCollector : IRuleResultCollector
    {
        private string _setName;

        public void BeginSet(string setName)
        {
            _setName = setName;
            Console.Out.WriteLine($"Start checks for {_setName}.");
        }

        public void CollectResult(IAuthenticodeRule rule, RuleResult result)
        {
            if (_setName == null)
            {
                throw new InvalidOperationException("Cannot collect results for an unknown set.");
            }

            switch (result)
            {
                case RuleResult.Skip:
                    Console.Out.WriteLine($"\tRule #{rule.RuleId} \"{rule.RuleName}\" was skipped because it was suppressed.");
                    break;
                case RuleResult.Fail:
                    Console.Out.WriteLine($"\tRule #{rule.RuleId} \"{rule.RuleName}\" failed.");
                    break;
                case RuleResult.Pass:
                    Console.Out.WriteLine($"\tRule #{rule.RuleId} \"{rule.RuleName}\" passed.");
                    break;
            }
        }

        public void CompleteSet()
        {
            Console.Out.WriteLine($"Complete checks for {_setName}.");
            _setName = null;
        }

        public void Flush()
        {
        }
    }
}