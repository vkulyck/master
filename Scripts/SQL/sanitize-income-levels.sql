USE TGSSFO
GO

UPDATE dbo.tblClient SET IncomeLevel = 
	CASE IncomeLevel
		WHEN 'EXTREMELY LOW INCOME' THEN 'Extremely Low Income'
		WHEN 'EXTREMELY LOW-INCOME' THEN 'Extremely Low Income'
		WHEN 'LOW INCOME' THEN 'Low Income'
		WHEN 'LOW-INCOME' THEN 'Low Income'
		WHEN 'MODERATE INCOME' THEN 'Moderate Income'
		WHEN 'MODERATE-INCOME' THEN 'Moderate Income'
		WHEN 'ABOVE MODERATE INCOME' THEN 'Above Moderate Income'
		WHEN 'ABOVE MODERATE-INCOME' THEN 'Above Moderate Income'
		WHEN '' THEN NULL
		ELSE IncomeLevel
	END
	select distinct incomelevel from tblclient