class SOP {
  constructor({
    title = "",
    description = "",
    departmentId = 0,
    reference = "",
    isAiGenerated = false,
    sopHazards = [],
    sopVersions = [],
    id = null,
  } = {}) {
    this.title = title;
    this.description = description;
    this.departmentId = departmentId;
    this.reference = reference;
    this.isAiGenerated = isAiGenerated;
    // Ensure sopHazards is always an array
    this.sopHazards = Array.isArray(sopHazards)
      ? sopHazards.map((hazard) => new SOPHazard(hazard))
      : [];
    // Map SOPVersions to ensure correct structure
    this.sopVersions = Array.isArray(sopVersions)
      ? sopVersions.map((version) => new SOPVersion(version))
      : [];
    this.id = id;
  }
}

class SOPVersion {
  constructor({
    sopId = null,
    version = 1,
    title = "",
    description = "",
    status = 1,
    authorId = null,
    approvedById = null,
    createDate = new Date().toISOString(),
    approvalDate = null,
    requestApprovalDate = null,
    sopSteps = [],
    sopHazards = [],
    id = null,
  } = {}) {
    this.sopId = sopId;
    this.version = version;
    this.title = title;
    this.description = description;
    this.status = status;
    this.authorId = authorId;
    this.approvedById = approvedById;
    this.createDate = this.validateDate(createDate);
    this.approvalDate = approvalDate ? this.validateDate(approvalDate) : null;
    this.requestApprovalDate = requestApprovalDate
      ? this.validateDate(requestApprovalDate)
      : null;
    // Ensure sopSteps is always an array
    this.sopSteps = Array.isArray(sopSteps) ? sopSteps : [];
    // Map SOPHazards to ensure correct structure
    this.sopHazards = Array.isArray(sopHazards)
      ? sopHazards.map((hazard) => new SOPHazard(hazard))
      : [];
    this.id = id;
  }

  // Helper to validate and normalize dates
  validateDate(date) {
    return isNaN(Date.parse(date)) ? new Date().toISOString() : date;
  }
}

class SOPHazard {
  constructor({
    sopVersionId = null,
    name = null,
    controlMeasure = null,
    riskLevel = null,
    id = null,
  } = {}) {
    this.sopVersionId = sopVersionId;
    this.name = name;
    this.controlMeasure = controlMeasure;
    this.riskLevel = riskLevel;
    this.id = id;
  }
}

export default SOP;
