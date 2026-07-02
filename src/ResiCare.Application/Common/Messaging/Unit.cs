namespace ResiCare.Application.Common.Messaging;

/// <summary>
/// Représente "aucune valeur de retour" (équivalent de void) dans un monde générique
/// où un handler renvoie toujours un Task&lt;T&gt;. Une commande qui ne renvoie rien
/// d'utile (Update, Archive) implémente donc ICommand&lt;Unit&gt;. (Idem MediatR.Unit.)
/// </summary>
public readonly record struct Unit
{
    public static readonly Unit Value = default;
}
