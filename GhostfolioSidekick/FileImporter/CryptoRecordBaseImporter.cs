﻿using GhostfolioSidekick.Ghostfolio.API;
using GhostfolioSidekick.Model;

namespace GhostfolioSidekick.FileImporter
{
	public abstract class CryptoRecordBaseImporter<T> : RecordBaseImporter<T>
	{
		protected CryptoRecordBaseImporter(IGhostfolioAPI api) : base(api)
		{
		}

		protected async Task<Money> GetCorrectUnitPrice(Money originalUnitPrice, SymbolProfile? symbol, DateTime date)
		{
			if (originalUnitPrice.Amount > 0)
			{
				return originalUnitPrice;
			}

			// GetPrice from Ghostfolio
			var price = await api.GetMarketPrice(symbol, date);
			return price;
		}

		protected async Task<SymbolProfile?> GetAsset(string assetName, Account account)
		{
			var mappedName = CryptoMapper.Instance.GetFullname(assetName);

			return await api.FindSymbolByIdentifier(
				new[] { mappedName, assetName },
				account.Balance.Currency,
				DefaultSetsOfAssetClasses.CryptoBrokerDefaultSetAssestClasses,
				DefaultSetsOfAssetClasses.CryptoBrokerDefaultSetAssetSubClasses);
		}
	}
}
