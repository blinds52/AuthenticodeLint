﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthenticodeLint.Rules
{
    public class RsaDsaPrimarySignatureRule : IAuthenticodeSignatureRule
    {
        public int RuleId { get; } = 10012;

        public string RuleName { get; } = "RSA/DSA Primary Signature";

        public string ShortDescription { get; } = "Primary signature should be RSA or DSA.";

        public RuleResult Validate(IReadOnlyList<ISignature> graph, SignatureLogger verboseWriter, CheckConfiguration configuration)
        {
            var primary = graph.SingleOrDefault();
            //There are zero signatures.
            if (primary == null)
            {
                return RuleResult.Fail;
            }
            var info = BitStrengthCalculator.CalculateStrength(primary.Certificate);
            if (info.AlgorithmName != PublicKeyAlgorithm.RSA && info.AlgorithmName != PublicKeyAlgorithm.DSA)
            {
                verboseWriter.LogSignatureMessage(primary, $"Primary signature should use RSA or DSA key but uses ${info.AlgorithmName.ToString()}");
                return RuleResult.Fail;
            }
            return RuleResult.Pass;
        }
    }
}
