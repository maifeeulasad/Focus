import React, { useState, useEffect } from "react";
import { readdir, readFile } from "fs";
import moment from "moment";

import styles from "styles/app.module.scss";

interface TypedDict {
  [key: string]: number;
}

const App: React.FC = () => {
  const [logFiles, setLogFiles] = useState<string[]>([]);
  const [processTime, setProcesstime] = useState<TypedDict>({ unknown: 0 });

  const format = "yyyy-MM-dd HH:mm:ss zzz";
  const dir = "D:\\focus\\";

  useEffect(() => {
    readdir(dir, (_, files) => {
      setLogFiles(files.map((file) => dir + file));
    });
  }, []);

  const measureTime = (nodes: NodeListOf<Element>) => {
    nodes.forEach((node: Element) => {
      const nextNode = node.nextElementSibling;
      if (node.tagName === "record" || node.tagName === "empty") {
        if (
          nextNode &&
          (nextNode.tagName === "record" || nextNode.tagName === "empty")
        ) {
          const processName = node.getElementsByTagName("process")[0]
            .textContent as string;
          const timeNodeText =
            node.getElementsByTagName("timestamp")[0].textContent;
          const timeNextNodeText =
            nextNode.getElementsByTagName("timestamp")[0].textContent;
          const timeNode = moment(timeNodeText, format);
          const timeNodeNext = moment(timeNextNodeText, format);
          const diff = timeNodeNext.diff(timeNode, "ms");
          console.log(processName, ' --- ', diff);
          if (processName in processTime) {
            setProcesstime((p) => ({
              ...p,
              [processName]: diff + processTime[processName],
            }));
          } else {
            setProcesstime((p) => ({ ...p, [processName]: diff }));
          }
        }
      }
    });
  };

  const parseXml = (xmlString: string) => {
    const parser = new DOMParser();
    const xmlDoc = parser.parseFromString(xmlString, "text/xml");

    measureTime(xmlDoc.querySelectorAll("start,record,empty"));
  };

  useEffect(() => {
    readFile(logFiles[0], "utf8", (_, data) => {
      parseXml(data);
    });
  }, [logFiles]);

  return (
    <div className={styles.app}>
      ping-pong
      <div>{JSON.stringify(processTime)}</div>
    </div>
  );
};

export default App;
