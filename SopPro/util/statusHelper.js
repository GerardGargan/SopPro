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

export function getStatusColour(identifier) {
  switch (identifier) {
    case 1:
      return "#3498db";
    case 2:
      return "#E67E22";
    case 3:
      return "#2ecc71";
    case 4:
      return "#7f8c8d";
    case 5:
      return "#c0392b";
    default:
      return "#7f8c8d";
  }
}
