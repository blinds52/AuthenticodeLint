﻿using System;
using System.Collections.Generic;

namespace AuthenticodeLint.Rules
{
    public class StrongKeyLengthRule : IAuthenticodeSignatureRule
    {
        private const int MIN_RSADSA_KEY_SIZE = 2048;

        public int RuleId { get; } = 10011;

        public string RuleName { get; } = "Strong Key Length";

        public string ShortDescription { get; } = "Validates the key length of a signing certificate.";

        public RuleResult Validate(IReadOnlyList<ISignature> graph, SignatureLogger verboseWriter, CheckConfiguration configuration)
        {
            var signatures = graph.VisitAll(SignatureKind.Any | SignatureKind.Any);
            var result = RuleResult.Pass;
            foreach (var signature in signatures)
            {
                var keyInfo = BitStrengthCalculator.CalculateStrength(signature.Certificate);
                switch (keyInfo.AlgorithmName)
                {
                    case PublicKeyAlgorithm.ECDSA:
                        if (keyInfo.BitSize == null)
                        {
                            verboseWriter.LogSignatureMessage(signature, "Signature uses ECDSA with an unknown curve.");
                            result = RuleResult.Fail;
                        }
                        //We don't actually check the key size for ECDSA since all known values are acceptable.
                        break;
                    case PublicKeyAlgorithm.RSA:
                        if (keyInfo.BitSize == null)
                        {
                            verboseWriter.LogSignatureMessage(signature, "Signature has an unknown RSA key size.");
                            result = RuleResult.Fail;
                        }
                        else if (keyInfo.BitSize < MIN_RSADSA_KEY_SIZE)
                        {
                            verboseWriter.LogSignatureMessage(signature, $"Signature uses a RSA key of size {keyInfo.BitSize} which is below the recommended {MIN_RSADSA_KEY_SIZE}.");
                            result = RuleResult.Fail;
                        }
                        break;
                    case PublicKeyAlgorithm.DSA:
                        if (keyInfo.BitSize == null)
                        {
                            verboseWriter.LogSignatureMessage(signature, "Signature has an unknown DSA key size.");
                            result = RuleResult.Fail;
                        }
                        else if (keyInfo.BitSize < MIN_RSADSA_KEY_SIZE)
                        {
                            //Effectively, 1024 is the max for a DSA key, so this will likely always fail.
                            verboseWriter.LogSignatureMessage(signature, $"Signature uses a DSA key of size {keyInfo.BitSize} which is below the recommended {MIN_RSADSA_KEY_SIZE}.");
                            result = RuleResult.Fail;
                        }
                        break;
                    case PublicKeyAlgorithm.Other:
                        goto default;
                    default:
                        verboseWriter.LogSignatureMessage(signature, $"Signature uses an unknown algorithm.");
                        result = RuleResult.Fail;
                        break;
                }
            }
            return result;
        }
    }
}
