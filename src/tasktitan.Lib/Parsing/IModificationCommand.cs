namespace TaskTitan.Lib.Parsing;

public interface IModificationCommand { } // Marker interface


public record SetAttributeCmd(string AttributeName, string RawValue) : IModificationCommand;
// Note: We store RawValue here. The service layer will parse dates/numbers/etc.,
// potentially using the DateParser or UDA definitions. Or, you could parse
// simple types like numbers here, but defer complex/contextual parsing (dates, UDAs)
// to the service layer after the attribute name is known. Keeping it simple
// with RawValue initially is often easier.

public record ClearAttributeCmd(string AttributeName) : IModificationCommand;

public record AddTagCmd(string TagName) : IModificationCommand;

public record RemoveTagCmd(string TagName) : IModificationCommand;