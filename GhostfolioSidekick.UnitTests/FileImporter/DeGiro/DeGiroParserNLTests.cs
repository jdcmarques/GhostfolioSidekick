using AutoFixture;
using FluentAssertions;
using GhostfolioSidekick.FileImporter.DeGiro;
using GhostfolioSidekick.Ghostfolio.API;
using GhostfolioSidekick.Model;
using Moq;

namespace GhostfolioSidekick.UnitTests.FileImporter.DeGiro
{
	public class DeGiroParserNLTests
	{
		readonly Mock<IGhostfolioAPI> api;

		public DeGiroParserNLTests()
		{
			api = new Mock<IGhostfolioAPI>();
		}

		[Fact]
		public async Task CanParseActivities_TestFiles_True()
		{
			// Arrange
			var parser = new DeGiroParserNL(api.Object);

			foreach (var file in Directory.GetFiles("./FileImporter/TestFiles/DeGiro/NL//", "*.csv", SearchOption.AllDirectories))
			{
				// Act
				var canParse = await parser.CanParseActivities(new[] { file });

				// Assert
				canParse.Should().BeTrue($"File {file}  cannot be parsed");
			}
		}

		[Fact]
		public async Task ConvertActivitiesForAccount_SingleDeposit_Converted()
		{
			// Arrange
			var parser = new DeGiroParserNL(api.Object);
			var fixture = new Fixture();

			var account = fixture.Build<Account>().With(x => x.Balance, Balance.Empty(DefaultCurrency.EUR)).Create();

			api.Setup(x => x.GetAccountByName(account.Name)).ReturnsAsync(account);

			// Act
			account = await parser.ConvertActivitiesForAccount(account.Name, new[] { "./FileImporter/TestFiles/DeGiro/NL//CashTransactions/single_deposit.csv" });

			// Assert
			account.Balance.Current(DummyPriceConverter.Instance).Should().BeEquivalentTo(new Money(DefaultCurrency.EUR, 43.17M, new DateTime(2023, 12, 29, 18, 47, 0, DateTimeKind.Utc)));
		}

		[Fact]
		public async Task ConvertActivitiesForAccount_SingleBuyEuro_Converted()
		{
			// Arrange
			var parser = new DeGiroParserNL(api.Object);
			var fixture = new Fixture();

			var asset = fixture.Build<SymbolProfile>().With(x => x.Currency, DefaultCurrency.EUR).Create();
			var account = fixture.Build<Account>().With(x => x.Balance, Balance.Empty(DefaultCurrency.EUR)).Create();

			api.Setup(x => x.GetAccountByName(account.Name)).ReturnsAsync(account);
			api.Setup(x => x.FindSymbolByIdentifier("IE00B3XXRP09", It.IsAny<Currency>(), It.IsAny<AssetClass[]>(),
				It.IsAny<AssetSubClass[]>(), true, false)).ReturnsAsync(asset);

			// Act
			account = await parser.ConvertActivitiesForAccount(account.Name,
				new[] { "./FileImporter/TestFiles/DeGiro/NL//BuyOrders/single_buy_euro.csv" });

			// Assert
			account.Balance.Current(DummyPriceConverter.Instance).Should().BeEquivalentTo(new Money(DefaultCurrency.EUR,
				21.70M, new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc)));
			account.Activities.Should().BeEquivalentTo(new[]
			{
				new Activity(
					ActivityType.Buy,
					asset,
					new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc),
					1,
					new Money(DefaultCurrency.EUR, 77.30M, new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc)),
					new[] { new Money(DefaultCurrency.EUR, 1, new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc)) },
					"Transaction Reference: [b7ab0494-1b46-4e2f-9bd2-f79e6c87cb5a] (Details: asset IE00B3XXRP09)",
					"b7ab0494-1b46-4e2f-9bd2-f79e6c87cb5a"
					)
			});
		}

		[Fact]
		public async Task ConvertActivitiesForAccount_SingleBuyEuroWholeNumber_Converted()
		{
			// Arrange
			var parser = new DeGiroParserNL(api.Object);
			var fixture = new Fixture();

			var asset = fixture.Build<SymbolProfile>().With(x => x.Currency, DefaultCurrency.EUR).Create();
			var account = fixture.Build<Account>().With(x => x.Balance, Balance.Empty(DefaultCurrency.EUR)).Create();

			api.Setup(x => x.GetAccountByName(account.Name)).ReturnsAsync(account);
			api.Setup(x => x.FindSymbolByIdentifier("IE00B3XXRP09", It.IsAny<Currency>(), It.IsAny<AssetClass[]>(),
				It.IsAny<AssetSubClass[]>(), true, false)).ReturnsAsync(asset);

			// Act
			account = await parser.ConvertActivitiesForAccount(account.Name,
				new[] { "./FileImporter/TestFiles/DeGiro/NL//BuyOrders/single_buy_euro_whole_number.csv" });

			// Assert
			account.Balance.Current(DummyPriceConverter.Instance).Should().BeEquivalentTo(new Money(DefaultCurrency.EUR,
				21.70M, new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc)));
			account.Activities.Should().BeEquivalentTo(new[]
			{
				new Activity(
					ActivityType.Buy,
					asset,
					new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc),
					1,
					new Money(DefaultCurrency.EUR, 77M, new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc)),
					new[] { new Money(DefaultCurrency.EUR, 1, new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc)) },
					"Transaction Reference: [b7ab0494-1b46-4e2f-9bd2-f79e6c87cb5a] (Details: asset IE00B3XXRP09)",
					"b7ab0494-1b46-4e2f-9bd2-f79e6c87cb5a"
					)
			});
		}

		[Fact]
		public async Task ConvertActivitiesForAccount_SingleBuyUSD_Converted()
		{
			// Arrange
			var parser = new DeGiroParserNL(api.Object);
			var fixture = new Fixture();

			var asset = fixture.Build<SymbolProfile>().With(x => x.Currency, DefaultCurrency.USD).Create();
			var account = fixture.Build<Account>().With(x => x.Balance, Balance.Empty(DefaultCurrency.USD)).Create();

			api.Setup(x => x.GetAccountByName(account.Name)).ReturnsAsync(account);
			api.Setup(x => x.FindSymbolByIdentifier("IE00B3XXRP09", It.IsAny<Currency>(), It.IsAny<AssetClass[]>(),
				It.IsAny<AssetSubClass[]>(), true, false)).ReturnsAsync(asset);

			// Act
			account = await parser.ConvertActivitiesForAccount(account.Name,
				new[] { "./FileImporter/TestFiles/DeGiro/NL//BuyOrders/single_buy_usd.csv" });

			// Assert
			account.Balance.Current(DummyPriceConverter.Instance).Should().BeEquivalentTo(new Money(DefaultCurrency.USD,
				21.70M, new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc)));
			account.Activities.Should().BeEquivalentTo(new[]
			{
				new Activity(
					ActivityType.Buy,
					asset,
					new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc),
					1,
					new Money(DefaultCurrency.USD, 77.30M, new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc)),
					new[] { new Money(DefaultCurrency.USD, 1, new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc)) },
					"Transaction Reference: [b7ab0494-1b46-4e2f-9bd2-f79e6c87cb5a] (Details: asset IE00B3XXRP09)",
					"b7ab0494-1b46-4e2f-9bd2-f79e6c87cb5a"
					)
			});
		}

		[Fact]
		public async Task ConvertActivitiesForAccount_SingleSellEuro_Converted()
		{
			// Arrange
			var parser = new DeGiroParserNL(api.Object);
			var fixture = new Fixture();

			var asset = fixture.Build<SymbolProfile>().With(x => x.Currency, DefaultCurrency.EUR).Create();
			var account = fixture.Build<Account>().With(x => x.Balance, Balance.Empty(DefaultCurrency.EUR)).Create();

			api.Setup(x => x.GetAccountByName(account.Name)).ReturnsAsync(account);
			api.Setup(x => x.FindSymbolByIdentifier("IE00B3XXRP09", It.IsAny<Currency>(), It.IsAny<AssetClass[]>(),
				It.IsAny<AssetSubClass[]>(), true, false)).ReturnsAsync(asset);

			// Act
			account = await parser.ConvertActivitiesForAccount(account.Name,
				new[] { "./FileImporter/TestFiles/DeGiro/NL//SellOrders/single_sell_euro.csv" });

			// Assert
			account.Balance.Current(DummyPriceConverter.Instance).Should().BeEquivalentTo(new Money(DefaultCurrency.EUR,
				21.70M, new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc)));
			account.Activities.Should().BeEquivalentTo(new[]
			{
				new Activity(
					ActivityType.Sell,
					asset,
					new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc),
					1,
					new Money(DefaultCurrency.EUR, 77.30M, new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc)),
					new[] { new Money(DefaultCurrency.EUR, 1, new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc)) },
					"Transaction Reference: [b7ab0494-1b46-4e2f-9bd2-f79e6c87cb5a] (Details: asset IE00B3XXRP09)",
					"b7ab0494-1b46-4e2f-9bd2-f79e6c87cb5a"
					)
			});
		}

		[Fact]
		public async Task ConvertActivitiesForAccount_SingleSellUSD()
		{
			// Arrange
			var parser = new DeGiroParserNL(api.Object);
			var fixture = new Fixture();

			var asset = fixture.Build<SymbolProfile>().With(x => x.Currency, DefaultCurrency.USD).Create();
			var account = fixture.Build<Account>().With(x => x.Balance, Balance.Empty(DefaultCurrency.USD)).Create();

			api.Setup(x => x.GetAccountByName(account.Name)).ReturnsAsync(account);
			api.Setup(x => x.FindSymbolByIdentifier("IE00B3XXRP09", It.IsAny<Currency>(), It.IsAny<AssetClass[]>(),
				It.IsAny<AssetSubClass[]>(), true, false)).ReturnsAsync(asset);

			// Act
			account = await parser.ConvertActivitiesForAccount(account.Name,
				new[] { "./FileImporter/TestFiles/DeGiro/NL//SellOrders/single_sell_usd.csv" });

			// Assert
			account.Balance.Current(DummyPriceConverter.Instance).Should().BeEquivalentTo(new Money(DefaultCurrency.USD,
				21.70M, new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc)));
			account.Activities.Should().BeEquivalentTo(new[]
			{
				new Activity(
					ActivityType.Sell,
					asset,
					new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc),
					1,
					new Money(DefaultCurrency.USD, 77.30M, new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc)),
					new[] { new Money(DefaultCurrency.USD, 1, new DateTime(2023, 07, 6, 9, 39, 0, DateTimeKind.Utc)) },
					"Transaction Reference: [b7ab0494-1b46-4e2f-9bd2-f79e6c87cb5a] (Details: asset IE00B3XXRP09)",
					"b7ab0494-1b46-4e2f-9bd2-f79e6c87cb5a"
					)
			});
		}

		[Fact]
		public async Task ConvertActivitiesForAccount_SingleBuyEuroMultipart_Converted()
		{
			// Arrange
			var parser = new DeGiroParserNL(api.Object);
			var fixture = new Fixture();

			var asset = fixture.Build<SymbolProfile>().With(x => x.Currency, DefaultCurrency.EUR).Create();
			var account = fixture.Build<Account>().With(x => x.Balance, Balance.Empty(DefaultCurrency.EUR)).Create();

			api.Setup(x => x.GetAccountByName(account.Name)).ReturnsAsync(account);
			api.Setup(x => x.FindSymbolByIdentifier("NL0011794037", It.IsAny<Currency>(), It.IsAny<AssetClass[]>(),
				It.IsAny<AssetSubClass[]>(), true, false)).ReturnsAsync(asset);

			// Act
			account = await parser.ConvertActivitiesForAccount(account.Name,
				new[] { "./FileImporter/TestFiles/DeGiro/NL//BuyOrders/single_buy_euro_multipart.csv" });

			// Assert
			account.Activities.Should().BeEquivalentTo(new[]
			{
				new Activity(
					ActivityType.Buy,
					asset,
					new DateTime(2023, 11, 10, 17, 10, 0, DateTimeKind.Utc),
					34,
					new Money(DefaultCurrency.EUR, 26.88M, new DateTime(2023, 11, 10, 17, 10, 0, DateTimeKind.Utc)),
					new[] { new Money(DefaultCurrency.EUR, 3, new DateTime(2023, 11, 10, 17, 10, 0, DateTimeKind.Utc)) },
					"Transaction Reference: [35d4345a-467c-42bd-848c-f6087737dd36] (Details: asset NL0011794037)",
					"35d4345a-467c-42bd-848c-f6087737dd36"
					),
				new Activity(
					ActivityType.Buy,
					asset,
					new DateTime(2023, 11, 10, 17, 10, 0, DateTimeKind.Utc),
					4,
					new Money(DefaultCurrency.EUR, 26.88M, new DateTime(2023, 11, 10, 17, 10, 0, DateTimeKind.Utc)),
					new Money[0],
					"Transaction Reference: [35d4345a-467c-42bd-848c-f6087737dd36 2] (Details: asset NL0011794037)",
					"35d4345a-467c-42bd-848c-f6087737dd36 2"
					)
			});
		}

		[Fact]
		public async Task ConvertActivitiesForAccount_SingleDividend_Converted()
		{
			// Arrange
			var parser = new DeGiroParserNL(api.Object);
			var fixture = new Fixture();

			var asset = fixture.Build<SymbolProfile>().With(x => x.Currency, DefaultCurrency.EUR).Create();
			var account = fixture.Build<Account>().With(x => x.Balance, Balance.Empty(DefaultCurrency.EUR)).Create();

			api.Setup(x => x.GetAccountByName(account.Name)).ReturnsAsync(account);
			api.Setup(x => x.FindSymbolByIdentifier("NL0009690239", It.IsAny<Currency>(), It.IsAny<AssetClass[]>(),
				It.IsAny<AssetSubClass[]>(), true, false)).ReturnsAsync(asset);

			// Act
			account = await parser.ConvertActivitiesForAccount(account.Name,
				new[] { "./FileImporter/TestFiles/DeGiro/NL//CashTransactions/single_dividend.csv" });

			// Assert
			account.Balance.Current(DummyPriceConverter.Instance).Should().BeEquivalentTo(new Money(DefaultCurrency.EUR,
				24.39M, new DateTime(2023, 09, 14, 6, 32, 0, DateTimeKind.Utc)));
			account.Activities.Should().BeEquivalentTo(new[]
			{
				new Activity(
					ActivityType.Dividend,
					asset,
					new DateTime(2023, 09, 14, 6, 32, 0, DateTimeKind.Utc),
					1,
					new Money(DefaultCurrency.EUR, 8.13M, new DateTime(2023, 09, 14, 6, 32, 0, DateTimeKind.Utc)),
					Enumerable.Empty<Money>(),
					"Transaction Reference: [Dividend_2023-09-14_06:32_NL0009690239] (Details: asset NL0009690239)",
					"Dividend_2023-09-14_06:32_NL0009690239"
					)
			});
		}

		[Fact]
		public async Task ConvertActivitiesForAccount_SingleDividendNoTax_Converted()
		{
			// Arrange
			var parser = new DeGiroParserNL(api.Object);
			var fixture = new Fixture();

			var asset = fixture.Build<SymbolProfile>().With(x => x.Currency, DefaultCurrency.EUR).Create();
			var account = fixture.Build<Account>().With(x => x.Balance, Balance.Empty(DefaultCurrency.EUR)).Create();

			api.Setup(x => x.GetAccountByName(account.Name)).ReturnsAsync(account);
			api.Setup(x => x.FindSymbolByIdentifier("NL0009690239", It.IsAny<Currency>(), It.IsAny<AssetClass[]>(),
				It.IsAny<AssetSubClass[]>(), true, false)).ReturnsAsync(asset);

			// Act
			account = await parser.ConvertActivitiesForAccount(account.Name,
				new[] { "./FileImporter/TestFiles/DeGiro/NL//CashTransactions/single_dividend_notax.csv" });

			// Assert
			account.Balance.Current(DummyPriceConverter.Instance).Should().BeEquivalentTo(new Money(DefaultCurrency.EUR,
				33.96M, new DateTime(2023, 09, 14, 6, 32, 0, DateTimeKind.Utc)));
			account.Activities.Should().BeEquivalentTo(new[]
			{
				new Activity(
					ActivityType.Dividend,
					asset,
					new DateTime(2023, 09, 14, 6, 32, 0, DateTimeKind.Utc),
					1,
					new Money(DefaultCurrency.EUR, 9.57M, new DateTime(2023, 09, 14, 6, 32, 0, DateTimeKind.Utc)),
					Enumerable.Empty<Money>(),
					"Transaction Reference: [Dividend_2023-09-14_06:32_NL0009690239] (Details: asset NL0009690239)",
					"Dividend_2023-09-14_06:32_NL0009690239"
					)
			});
		}
	}
}