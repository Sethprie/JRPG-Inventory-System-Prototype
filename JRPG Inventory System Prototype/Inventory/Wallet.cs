using System;
using System.Collections.Generic;
using System.Text;

namespace JRPG_Inventory_System_Prototype;

public class Wallet
{
    public int Gold { get; private set; }

    public Wallet(int gold = 0) => Gold = gold;

    public void Add(int amount) => Gold += amount;

    public bool TrySpend(int amount)
    {
        if (amount > Gold) return false;
        Gold -= amount;
        return true;
    }

    public void Set(int amount) => Gold = amount;
}