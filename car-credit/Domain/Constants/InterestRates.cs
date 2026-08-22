using CarCredit.Domain.Enums;

namespace CarCredit.Domain.Constants;

public static class InterestRates
{
    private static readonly IReadOnlyDictionary<EInstallmentsTerm, decimal> RatesByTerm =
        new Dictionary<EInstallmentsTerm, decimal>
        {
            { EInstallmentsTerm.Months6, 0.015m },
            { EInstallmentsTerm.Months12, 0.028m },
            { EInstallmentsTerm.Months18, 0.042m },
            { EInstallmentsTerm.Months24, 0.068m },
            { EInstallmentsTerm.Months30, 0.082m },
            { EInstallmentsTerm.Months36, 0.115m },
            { EInstallmentsTerm.Months42, 0.154m },
            { EInstallmentsTerm.Months48, 0.183m }
        };

    public static decimal For(EInstallmentsTerm term)
    {
        if (!RatesByTerm.TryGetValue(term, out decimal rate))
            throw new KeyNotFoundException(
                $"No existe una tasa de interés configurada para el plazo de {(int)term} meses.");

        return rate;
    }
}