CREATE PROC GetChildHid
	@parent_id BIGINT,
	@child_hid HIERARCHYID OUT
AS
BEGIN
	DECLARE @parenthid HIERARCHYID, @maxChildHid HIERARCHYID;

	SELECT @parenthid = employee_hid FROM Employees 
			WHERE employee_id = @parent_id;

	SELECT @maxChildHid = MAX(employee_hid)
			FROM Employees 
			WHERE employee_hid.GetAncestor(1) =  @parenthid;

	SET @child_hid = @parenthid.GetDescendant(@maxChildHid, null);
END