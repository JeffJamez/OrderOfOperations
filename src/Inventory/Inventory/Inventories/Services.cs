namespace Inventory.Inventories;

public static class Services
{
    public delegate ValueTask<bool> IsInventoryAvailableBySku(Sku sku);

    public delegate ValueTask<bool> IsBillOfLadingValid(string billOfLadingId);
}
