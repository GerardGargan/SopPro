import { useState } from "react";
import { downloadSopVersion } from "../util/downloadHelper";

export default downloadSop = () => {
  const [isDownloading, setIsDownloading] = useState(false);
  const [isSuccessful, setIsSuccessful] = useState(false);
  const [isError, setIsError] = useState(false);

  function resetState() {
    setIsDownloading(false);
    setIsSuccessful(false);
    setIsError(false);
  }

  const handleDownload = async (versionId, reference, title) => {
    setIsDownloading(true);
    setIsSuccessful(false);
    setIsError(false);

    try {
      await downloadSopVersion(versionId, reference, title);
      setIsSuccessful(true);
    } catch (e) {
      setIsError(true);
    }
    setIsDownloading(false);
  };

  return {
    isDownloading,
    isSuccessful,
    isError,
    handleDownload,
    resetState,
  };
};
