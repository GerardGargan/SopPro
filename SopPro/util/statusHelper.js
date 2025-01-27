export function getStatus(identifier) {
  switch (identifier) {
    case 1:
      return "Draft";
    case 2:
      return "In Review";
    case 3:
      return "Approved";
    case 4:
      return "Archived";
    case 5:
      return "Rejected";
    default:
      return "Unknown";
  }
}
