import { useState } from "react";
import { downloadSopVersion } from "../util/downloadHelper";

export default downloadSop = () => {
  const [isDownloading, setIsDownloading] = useState(false);
  const [isSuccessful, setIsSuccessful] = useState(false);
  const [isError, setIsError] = useState(false);

  const handleDownload = async ({ id, reference, title }) => {
    setIsDownloading(true);
    setIsSuccessful(false);
    setIsError(false);

    try {
      await downloadSopVersion(id, reference, title);
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
  };
};
