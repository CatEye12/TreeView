CREATE PROCEDURE [dbo].[AddFirstChildNode]
	@parent_name NVARCHAR, 
	@child_name NVARCHAR(50)
	/*@child_hid HIERARCHYID out*/
AS
BEGIN
	DECLARE @parent_hid HIERARCHYID, @child_hid HIERARCHYID;

	SELECT @parent_hid = employee_hid FROM Employees WHERE Employees.name = @parent_name;

	SET @child_hid = @parent_hid.GetDescendant(NULL, NULL);

	INSERT INTO Employees
	OUTPUT
		INSERTED.employee_id,
		INSERTED.employee_hid,
		INSERTED.employee_hid.ToString() AS employee_hid_string,
		INSERTED.name
	VALUES(@child_hid, @child_name);
END